using System.Runtime.CompilerServices;
using System.IO;
using System.Text;
using System.Data;
using System.Net.Sockets;
using System.Net;
using System;
using Microsoft.VisualBasic.ApplicationServices;
using static System.Windows.Forms.AxHost;
using System.Drawing;
using System.Diagnostics.Eventing.Reader;
using _240701_5TreeMake;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Net.Http;
using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;
using System.Diagnostics;
using ACTMULTILib;
using System.Numerics;
namespace _240701_5TreeMake
{
    public enum STONE { none, black, white };
    public partial class Form1 : Form
    {
        Graphics g;
        Pen pen;
        Brush wBrush, bBrush;
        int margin = 40;
        int 눈size = 30;
        int 돌size = 28;
        int 화점size = 10;
        public STONE[,] 바둑판 = new STONE[19, 19];
        public bool flag = false;//false =검은돌, true=흰돌
        public bool imageFlag = true;
        private bool sequenceFlag = true;
        int stoneCnt = 1; //수순
        Font font = new Font("맑은 고딕", 6);

        int sequence = 0; //복기에 사용되는 순서
        bool reviveFlag = false; //복기 모드인지 알리는 플래그

        List<Revive> lstRevive = new List<Revive>(); //리스트
        private string dirName; //게임을 저장하기 위한 디렉토리 이름

        bool tcpMitiFlag = false; //TCP연결을 위한 플래그
        bool tcpListenerFlag = false;//서버열었니?
        TcpListener tcpListener;//tcp리스너
        TcpListener chatListener;//채팅 리스너
        TcpClient chatClient;//채팅 클라이언트
        TcpClient tcpClient;//클라이언트
        NetworkStream stream;
        NetworkStream chatStream;
        int tcpCount = 0;
        bool gameReady = false;
        bool gameReadyTo = false;
        string gameID = "";
        string gameIDTo = "";

        bool turn = false;//차례 본인 true, 타인 false
        DateTime clickTime = DateTime.Now.AddDays(7);
        DateTime receiveTime = DateTime.Now.AddDays(7);


        bool plcFlag = false;
        ActEasyIF plc1;



        public Form1()
        {
            InitializeComponent();

            panel1.BackColor = Color.Orange;

            pen = new Pen(Color.Black);
            bBrush = new SolidBrush(Color.Black);
            wBrush = new SolidBrush(Color.White);
            ClientSize = new Size(2 * margin + 18 * 눈size + 30 + 600, 2 * margin + 18 * 눈size + 60);
            //panel1.ClientSize = new Size(2 * margin + 18 * 눈size, 2 * margin + 18 * 눈size);


        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            DrawBord();
            DrawStones();
        }
        private void DrawStones()
        {
            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    if (바둑판[x, y] == STONE.white)
                        if (imageFlag == false)
                            g.FillEllipse(wBrush, margin + x * 눈size - 돌size / 2,
                                margin + y * 눈size - 돌size / 2, 돌size, 돌size);
                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/white.png");
                            g.DrawImage(bmp, margin + x * 눈size - 돌size / 2, margin + y * 눈size - 돌size / 2, 돌size, 돌size);
                        }
                    else if (바둑판[x, y] == STONE.black)
                        if (imageFlag == false)
                            g.FillEllipse(bBrush, margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);
                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/black.png");
                            g.DrawImage(bmp, margin + x * 눈size - 돌size / 2, margin + y * 눈size - 돌size / 2, 돌size, 돌size);
                        }
        }
        private void DrawBord()
        {
            g = panel1.CreateGraphics();
            //바둑판 선 그리기
            for (int i = 0; i < 19; i++)
            {
                g.DrawLine(pen, new Point(margin + i * 눈size, margin),
                    new Point(margin + i * 눈size, margin + 18 * 눈size));
                g.DrawLine(pen, new Point(margin, margin + i * 눈size),
                    new Point(margin + 18 * 눈size, margin + i * 눈size));
            }

            //화점그리기(점9개 그리기)
            for (int x = 3; x <= 15; x += 6)
                for (int y = 3; y <= 15; y += 6)
                {
                    g.FillEllipse(bBrush,
                        margin + 눈size * x - 화점size / 2,
                        margin + 눈size * y - 화점size / 2,
                        화점size, 화점size);
                }
        }
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            plc1.SetDevice("M2", 0);//리셋신호 초기화
        }
        //마우스 클릭 변경하기
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (plcFlag==true) panelPLC_MouseDown(sender, e);//싱글에서 PLC 연동
            else
            {
                if (tcpMitiFlag == false) panelSingle_MouseDown(sender, e);  //싱글 모드일 경우
                else panelMiti_MouseDown(sender, e);//멀티 모드일 경우
            }
        }
        //m1은 연결 할때, m0은 움직이고 있는지 무브 확인,m2는 에러리셋 d1은 x, d2은 y
        //m3은 움직이라고 명령
        private void panelPLC_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (reviveFlag == true)
            {
                ReviveGame();
                return;
            }
            //e.x는 픽셀단위, x는 바둑판 좌표
            int x = (e.X - margin + 눈size / 2) / 눈size;
            int y = (e.Y - margin + 눈size / 2) / 눈size;
            if (x > 18 || x < 0) return;//바둑판 밖을 클릭할 경우
            if (y > 18 || y < 0) return;
            if (바둑판[x, y] != STONE.none) return;//클릭한 곳을 클릭할 경우


            // 바둑판[x,y]에 돌을 그리기 위한 REctangle
            Rectangle r = new Rectangle(margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);
            try
            {
                if (flag == false)//검은돌차례
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//상대경로익히기
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    바둑판[x, y] = STONE.black;
                    plc1.SetDevice("M24", 1);//검은돌

                }
                else//흰돌차례
                {
                    if (imageFlag == false) g.FillEllipse(wBrush, r);
                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/white.png");
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.Black, r);
                    lstRevive.Add(new Revive(x, y, STONE.white, stoneCnt++));
                    flag = false;
                    바둑판[x, y] = STONE.white;
                    plc1.SetDevice("M34", 1);//흰돌
                }
            }
            catch (ArgumentException ex)
            {
                // 예외 처리: 매개 변수가 잘못되었을 때 실행할 코드 작성
                MessageBox.Show("이미지 파일 경로가 잘못되었습니다.", ex.Message);
                // 실제 예외 메시지 출력 (디버깅용)
            }
            catch (Exception ex)
            {
                // 기타 예외 처리
                MessageBox.Show("예기치 않은 오류가 발생했습니다.", ex.Message);
                // 실제 예외 메시지 출력 (디버깅용)
            }

            plc1.SetDevice("D5", x);//값 보내기
            plc1.SetDevice("D2", y);
            plc1.SetDevice("M3", 1);//움직여!
            CkeckOmok(x, y);

        }

        private async void panelMiti_MouseDown(object sender, MouseEventArgs e)
        {
            if (gameReady == false || gameReadyTo == false)
            {
              
                MessageBox.Show("준비 버튼을 누르세요.");
                return;
            }

            if (turn == true)
            {

                //e.x는 픽셀단위, x는 바둑판 좌표
                int x = (e.X - margin + 눈size / 2) / 눈size;
                int y = (e.Y - margin + 눈size / 2) / 눈size;
                if (x > 18 || x < 0) return;//바둑판 밖을 클릭할 경우
                if (y > 18 || y < 0) return;
                if (바둑판[x, y] != STONE.none) return;//클릭한 곳을 클릭할 경우
                //TimeCheck();
                // 바둑판[x,y]에 돌을 그리기 위한 REctangle
                Rectangle r = new Rectangle(margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);

                if (tcpListenerFlag == true)//검은돌차례//서버
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//상대경로익히기
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    바둑판[x, y] = STONE.black;
                }
                else//흰돌차례//클라
                {
                    if (imageFlag == false) g.FillEllipse(wBrush, r);
                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/white.png");
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.Black, r);
                    lstRevive.Add(new Revive(x, y, STONE.white, stoneCnt++));
                    flag = false;
                    바둑판[x, y] = STONE.white;
                }
                string xy = x + "," + y;
                byte[] buffer = Encoding.UTF8.GetBytes(xy);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                turn = false;
                //누르고 받기까지
                clickTime = DateTime.Now;//시간초과
                CkeckOmok(x, y);

            }
        }
        private async void opponentPlayer()
        {
            while (true)
            {
                //TimeCheck();
                byte[] buffer = new byte[100];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);//읽기

                byte[] receivedBytes = new byte[bytesRead];
                Array.Copy(buffer, receivedBytes, bytesRead);//배열크기 조정

                string receivedString = Encoding.UTF8.GetString(receivedBytes);
                string[] xyStrings = receivedString.Split(',');

                int x = int.Parse(xyStrings[0]);
                int y = int.Parse(xyStrings[1]);
                Rectangle r = new Rectangle(margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);
                if (turn == false)
                {
                    if (tcpListenerFlag == true)//서버일때, 흰돌 받기
                    {
                        if (imageFlag == false) g.FillEllipse(wBrush, r);
                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/white.png");
                            g.DrawImage(bmp, r);
                        }
                        DrawStonesSequence(stoneCnt, Brushes.Black, r);
                        lstRevive.Add(new Revive(x, y, STONE.white, stoneCnt++));
                        flag = false;
                        바둑판[x, y] = STONE.white;
                    }
                    else//클라일때, 검은 돌 받기
                    {
                        if (imageFlag == false) g.FillEllipse(bBrush, r);

                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/black.png");//상대경로익히기
                            g.DrawImage(bmp, r);
                        }
                        DrawStonesSequence(stoneCnt, Brushes.White, r);
                        lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                        flag = true;
                        바둑판[x, y] = STONE.black;
                    }
                    turn = true;

                    CkeckOmok(x, y);
                    receiveTime = DateTime.Now; //시간초과//받고누르기까지

                }
            }
        }

        private void TimeCheck()
        {
            TimeSpan rs = DateTime.Now - receiveTime;
            double rsec = (double)(rs.Ticks / 10000000.0);
            MessageBox.Show(rsec.ToString());
            if (rsec >= 10.0)
            {
                saveGame();
                DialogResult res = MessageBox.Show($"시간 초과\r\n {gameIDTo}님이 승리하셨습니다.",
                    "게임종료", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                    NewGame();
                else if (res == DialogResult.No)
                    this.Close();
            }
            TimeSpan cs = DateTime.Now - receiveTime;
            double csec = (double)(cs.Ticks / 10000000.0);
            if (csec >= 10.0)
            {
                saveGame();
                DialogResult res = MessageBox.Show($"시간 초과\r\n {gameID}님이 승리하셨습니다.");
                if (res == DialogResult.Yes)
                    NewGame();
                else if (res == DialogResult.No)
                    this.Close();
            }
        }

        private void panelSingle_MouseDown(object sender, MouseEventArgs e)
        {
            if (reviveFlag == true)
            {
                ReviveGame();
                return;
            }
            //e.x는 픽셀단위, x는 바둑판 좌표
            int x = (e.X - margin + 눈size / 2) / 눈size;
            int y = (e.Y - margin + 눈size / 2) / 눈size;
            if (x > 18 || x < 0) return;//바둑판 밖을 클릭할 경우
            if (y > 18 || y < 0) return;
            if (바둑판[x, y] != STONE.none) return;//클릭한 곳을 클릭할 경우


            // 바둑판[x,y]에 돌을 그리기 위한 REctangle
            Rectangle r = new Rectangle(margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);
            try
            {
                if (flag == false)//검은돌차례
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//상대경로익히기
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    바둑판[x, y] = STONE.black;
                }
                else//흰돌차례
                {
                    if (imageFlag == false) g.FillEllipse(wBrush, r);
                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/white.png");
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.Black, r);
                    lstRevive.Add(new Revive(x, y, STONE.white, stoneCnt++));
                    flag = false;
                    바둑판[x, y] = STONE.white;
                }
            }
            catch (ArgumentException ex)
            {
                // 예외 처리: 매개 변수가 잘못되었을 때 실행할 코드 작성
                MessageBox.Show("이미지 파일 경로가 잘못되었습니다.", ex.Message);
                // 실제 예외 메시지 출력 (디버깅용)
            }
            catch (Exception ex)
            {
                // 기타 예외 처리
                MessageBox.Show("예기치 않은 오류가 발생했습니다.", ex.Message);
                // 실제 예외 메시지 출력 (디버깅용)
            }
            CkeckOmok(x, y);
        }
        private void ReviveGame()
        {
            if (sequence < lstRevive.Count) DrawAStone(lstRevive[sequence++]);
        }
        private void DrawAStone(Revive item)
        {
            int x = item.X;
            int y = item.Y;
            STONE s = item.Stone;
            int seq = item.Seq;

            Rectangle r = new Rectangle(margin + 눈size * x - 돌size / 2, margin + 눈size * y - 돌size / 2, 돌size, 돌size);
            if (s == STONE.black)
            {
                if (imageFlag == false)
                    g.FillEllipse(bBrush, r);
                else
                {
                    Bitmap bmp = new Bitmap("../../../Images/black.png");
                    g.DrawImage(bmp, r);
                }
                DrawStonesSequence(seq, Brushes.White, r);
                바둑판[x, y] = STONE.black;
            }
            else
            {
                if (imageFlag == false) g.FillEllipse(bBrush, r);
                else
                {
                    Bitmap bmp = new Bitmap("../../../Images/white.png");
                    g.DrawImage(bmp, r);
                }
                DrawStonesSequence(seq, Brushes.Black, r);
                바둑판[x, y] = STONE.white;
            }
            CkeckOmok(x, y);

        }
        private void DrawStonesSequence(int v, Brush color, Rectangle r)
        {
            if (sequenceFlag == true)
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.Alignment = StringAlignment.Center;
                stringFormat.LineAlignment = StringAlignment.Center;
                g.DrawString(v.ToString(), font, color, r, stringFormat);
            }
        }
        private void CkeckOmok(int x, int y)
        {
            //수평
            int cnt = 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++)
                if (바둑판[x, y] == 바둑판[i, y]) cnt++;
                else break;
            for (int i = x - 1; i >= 0 && i <= 18; i--)
                if (바둑판[x, y] == 바둑판[i, y]) cnt++;
                else break;

            //수직
            int vcnt = 1;
            for (int i = y + 1; i >= 0 && i <= 18; i++)
                if (바둑판[x, y] == 바둑판[x, i]) vcnt++;
                else break;
            for (int i = y - 1; i >= 0 && i <= 18; i--)
                if (바둑판[x, y] == 바둑판[x, i]) vcnt++;
                else break;

            //대각선
            int rcnt = 1;
            int yy = y + 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++, yy++)
            {
                if (yy < 0 || yy > 18) break;
                if (바둑판[x, y] == 바둑판[i, yy]) rcnt++;
                else break;
            }
            yy = y - 1;
            for (int i = x - 1; i >= 0 && i <= 18; i--, yy--)
            {
                if (yy < 0 || yy > 18) break;
                if (바둑판[x, y] == 바둑판[i, yy]) rcnt++;
                else break;
            }

            //역대각선
            int lcnt = 1;
            yy = y - 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++, yy--)
            {
                if (yy < 0 || yy > 18) break;
                if (바둑판[x, y] == 바둑판[i, yy]) lcnt++;
                else break;
            }

            yy = y + 1;
            for (int i = x - 1; i >= 0 && i <= 18; i--, yy++)
            {
                if (yy < 0 || yy > 18) break;
                if (바둑판[x, y] == 바둑판[i, yy]) lcnt++;
                else break;
            }

            if (cnt >= 5 || vcnt >= 5 || rcnt >= 5 || lcnt >= 5)
            {
                OmokComplete(x, y);
                return;
            }
        }
        private void OmokComplete(int x, int y)
        {
            saveGame();
            DialogResult res = MessageBox.Show(바둑판[x, y].ToString().ToUpper()
                + "Wins!\n새로운 게임을 시작하시오", "게임종료", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
                NewGame();
            else if (res == DialogResult.No)
                this.Close();

        }
        private void saveGame()
        {
            if (reviveFlag == true)
                return;//복기 모드에서는 저장하지 않음

            string documentPath = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents").ToString();
            dirName = documentPath + "/Omok/";

            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            string fileName = dirName + DateTime.Now.ToShortDateString() + "-" +
                DateTime.Now.Hour + DateTime.Now.Minute + ".omk";
            FileStream fs = new FileStream(fileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);

            foreach (Revive item in lstRevive)
            {
                sw.WriteLine("{0} {1} {2} {3}", item.X, item.Y, item.Stone, item.Seq);
            }
            sw.Close();
            fs.Close();
        }
        public void NewGame()
        {
            InitializeOmok();
        }
        private void 다시시작ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeOmok();
        }
        private void InitializeOmok()
        {
            복기ToolStripMenuItem.Checked = false;
            imageFlag = true;//이미지?그리기?
            flag = false;//false =검은돌, true=흰돌
            sequenceFlag = true;
            stoneCnt = 1;//수순
            reviveFlag = false;//복기 모드인지 알리는 플래그
            sequence = 0; //복기에 사용되는 순서
            clickTime = DateTime.Now.AddDays(7);
            receiveTime = DateTime.Now.AddDays(7);
            for (int x = 0; x < 19; x++)//바둑판 흑백 배열 초기화
                for (int y = 0; y < 19; y++)
                    바둑판[x, y] = STONE.none;
            lstRevive.Clear(); //리스트
            panel1.Refresh();//판넬 초기화
            DrawBord();//바둑판그리기

            if (plcFlag==true)
            {
                plc1.SetDevice("M1", 0);
                Thread.Sleep(500);
                plc1.SetDevice("M1", 1);
            }
            
        }
        private void 이미지ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageFlag = true;
            이미지ToolStripMenuItem.Checked = true;
            그리기ToolStripMenuItem.Checked = false;
        }
        private void 그리기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageFlag = false;
            그리기ToolStripMenuItem.Checked = true;
            이미지ToolStripMenuItem.Checked = false;
        }
        private void 수순표시ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sequenceFlag = true;
            수순표시ToolStripMenuItem.Checked = true;
            수순표시안함ToolStripMenuItem.Checked = false;
        }
        private void 수순표시안함ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sequenceFlag = false;
            수순표시안함ToolStripMenuItem.Checked = true;
            수순표시ToolStripMenuItem.Checked = false;
        }
        private void 복기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            복기ToolStripMenuItem.Checked = true;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = dirName;
            ofd.Filter = "Omok file(*.omk)|*.omk";
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            sequenceFlag = true;

            InitializeOmok();//현재 게임 중이라면 초기화
            try
            {
                StreamReader r = File.OpenText(fileName);
                string line = "";

                //파일 내용을 한줄씩 읽어서 lstRevive 리스트에 넣는다.
                while ((line = r.ReadLine()) != null)
                {
                    string[] item = line.Split(' ');
                    Revive rev = new Revive(
                        int.Parse(item[0]), int.Parse(item[1]),
                        item[2] == "black" ? STONE.black : STONE.white,
                        int.Parse(item[3])
                        );
                    lstRevive.Add(rev);
                }
                r.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            reviveFlag = true;//복기 준비
            sequence = 0;//복기 수순 초기화
        }
        private void Clear바둑판()
        {
            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    바둑판[x, y] = STONE.none;
        }
        private void 싱글모드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tcpMitiFlag = false;
            싱글모드ToolStripMenuItem.Checked = true;
            멀티모드ToolStripMenuItem.Checked = false;

            stream.Close();
            chatStream.Close();
            tcpClient.Close();
            chatClient.Close();
            tcpListener.Stop();
            chatListener.Stop();
        }
        private void 멀티모드ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("멀티모드로 변경하시겠습니까?" +
                "\r\n 현재 진행 중이던 내용은 모두 초기화됩니다.", "모드변경", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                tcpMitiFlag = true;
                멀티모드ToolStripMenuItem.Checked = true;
                싱글모드ToolStripMenuItem.Checked = false;
                panel1.MouseDown -= panel1_MouseDown;//연결 시 다시 추가
                InitializeOmok();//내용 초기화
            }
            else if (res == DialogResult.No)
                return;
        }
        //***********************************************************수정하기***********************************************************
        //1. 서버 열기
        //2. 연결하기
        //3. 데이터 주고 받기
        // 순으로 진행 


        //내가 서버일 경우
        private void 서버생성ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            tcpListenerFlag = true;//서버란 표시
            서버생성ToolStripMenuItem.Checked = true;
        }
        //생성 후 준비 신호 받을 때까지 대기

        //연결 버튼을 누를때
        private async void btnConnect_Click(object sender, EventArgs e)
        {

            if (tcpMitiFlag == false) return;//멀티모드가 아닐경우 비활성화
                                             //ip,설정부분
                                             //ip연결 후 레디 신호 보내기
            if (tcpListenerFlag == true)
            {
                tcpListener = new TcpListener(IPAddress.Parse(tbIP.Text), 50000);//서버일 경우
                tcpListener.Start();
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                idExchange();//아이디 교환

                chatListener = new TcpListener(IPAddress.Parse(tbIP.Text), 50001);
                chatListener.Start();
                chatClient = await chatListener.AcceptTcpClientAsync();
            }
            else
            {
                tcpClient = new TcpClient();//클라이언트일경우
                await tcpClient.ConnectAsync(tbIP.Text, 50000);//클라이언트일 경우
                idExchange();//아이디 교환

                chatClient = new TcpClient();
                await chatClient.ConnectAsync(tbIP.Text, 50001);

            }
            chatStream = chatClient.GetStream();
            chating();




        }
        private async void idExchange()//아이디 교환
        {
            bool connectSuccess = false;
            stream = tcpClient.GetStream();
            if (tcpListenerFlag == true)
            {
                //아이디 받음
                byte[] buffer = new byte[256];
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                //인코딩 후 넣기
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                gameIDTo = message;
                gameID = tbID.Text;
                lbChat.Items.Add(message + "님이 입장하셨습니다.");

                //아이디 보내기
                buffer = new byte[256];
                string text = tbID.Text;
                read = text.Length;
                buffer = BitConverter.GetBytes(read);
                stream.Write(Encoding.UTF8.GetBytes(text));
                connectSuccess = true;
            }
            else
            {
                //아이디 보냄
                byte[] buffer = new byte[256];
                string text = tbID.Text;
                int read = text.Length;
                buffer = BitConverter.GetBytes(read);
                stream.Write(Encoding.UTF8.GetBytes(text));

                //아이디 받음
                buffer = new byte[256];
                read = await stream.ReadAsync(buffer, 0, buffer.Length);
                //인코딩 후 넣기
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                lbChat.Items.Add(message + "님 방에 입장하셨습니다.");
                gameIDTo = message;
                gameID = tbID.Text;
                connectSuccess = true;

            }
            if (connectSuccess == true)
                panel1.MouseDown += panel1_MouseDown;//연결 성공시 다시 동작  
        }


        private async void btnReady_Click(object sender, EventArgs e)
        {

            stream = tcpClient.GetStream();
            byte[] readBuffer = new byte[1];
            byte[] writeBuffer = new byte[1];
            gameReady = true;
            if (tcpListenerFlag == true)
            {
                _ = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                if (readBuffer[0] == 1) gameReadyTo = true;

                if (gameReady == true) writeBuffer[0] = 1;
                await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);
                lbChat.Items.Add($"{gameIDTo}님이 준비를 완료하셨습니다.");
                turn = true;
            }
            else
            {
                if (gameReady == true) writeBuffer[0] = 1;
                await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);

                _ = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                if (readBuffer[0] == 1) gameReadyTo = true;
                lbChat.Items.Add($"{gameIDTo}님이 준비를 완료하셨습니다.");
                turn = false;
            }
            opponentPlayer();//상대방 턴일때 돌 받기를 대기하기


        }
        private async void chating()
        {

            while (true)//읽어서 쓰는 부분
            {
                byte[] buffer = new byte[1024];
                int read = await chatStream.ReadAsync(buffer, 0, buffer.Length);

                byte[] receivedBytes = new byte[read];
                Array.Copy(buffer, receivedBytes, read);

                string receivedString = Encoding.UTF8.GetString(receivedBytes);
                lbChat.Items.Add($"[{gameIDTo}] : {receivedString}");
            }

        }


        private async void button3_Click(object sender, EventArgs e)
        {
            string message = tbTalk.Text;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await chatStream.WriteAsync(buffer, 0, buffer.Length);
            lbChat.Items.Add($"[{gameID}] : {message}");
            tbTalk.Clear();
        }

        private async void tbTalk_Enter(object sender, EventArgs e)
        {
            string message = tbTalk.Text;
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await chatStream.WriteAsync(buffer, 0, buffer.Length);
            lbChat.Items.Add($"[{gameID}] : {message}");
            tbTalk.Clear();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (tcpListenerFlag)
                {
                    if (stream != null) stream.Close();
                    if (chatStream != null) chatStream.Close();
                    if (tcpClient != null) tcpClient.Close();
                    if (chatClient != null) chatClient.Close();
                    if (tcpListener != null) tcpListener.Stop();
                    if (chatListener != null) chatListener.Stop();
                }
                else
                {
                    if (stream != null) stream.Close();
                    if (chatStream != null) chatStream.Close();
                    if (tcpClient != null) tcpClient.Close();
                    if (chatClient != null) chatClient.Close();
                }
            }
            catch (Exception ex)
            {
                Application.ExitThread();
                Application.Exit();
            }

        }

        private void pLC연결ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pLC연결ToolStripMenuItem.Checked = true;
            pLC연결해제ToolStripMenuItem.Checked = false;
            plcFlag = true;
            plc1 = new ActEasyIF();
            plc1.ActLogicalStationNumber = 1;
            int conErr = plc1.Open();

            if (conErr == 0)
            {
                lbChat.Items.Add("PLC에 연결하셨습니다.");
                tmrUpdate.Start();
                plc1.SetDevice("M1", 1);
                plc1.SetDevice("M24", 0);
                plc1.SetDevice("M34", 0);

            }
            else
            {
                lbChat.Items.Add("연결 에러" + conErr);
            }
        }

        private void pLC연결해제ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pLC연결해제ToolStripMenuItem.Checked = true;
            pLC연결ToolStripMenuItem.Checked = false;
            plcFlag = false;

            plc1.SetDevice("M1", 0);
            plc1.Close();
            lbChat.Items.Add("연결이 해제되었습니다.");
            tmrUpdate.Stop();
        }

        private void 에러리셋ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plc1.SetDevice("M2", 1);
        }
    }
}

