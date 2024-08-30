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
        int ��size = 30;
        int ��size = 28;
        int ȭ��size = 10;
        public STONE[,] �ٵ��� = new STONE[19, 19];
        public bool flag = false;//false =������, true=��
        public bool imageFlag = true;
        private bool sequenceFlag = true;
        int stoneCnt = 1; //����
        Font font = new Font("���� ���", 6);

        int sequence = 0; //���⿡ ���Ǵ� ����
        bool reviveFlag = false; //���� ������� �˸��� �÷���

        List<Revive> lstRevive = new List<Revive>(); //����Ʈ
        private string dirName; //������ �����ϱ� ���� ���丮 �̸�

        bool tcpMitiFlag = false; //TCP������ ���� �÷���
        bool tcpListenerFlag = false;//����������?
        TcpListener tcpListener;//tcp������
        TcpListener chatListener;//ä�� ������
        TcpClient chatClient;//ä�� Ŭ���̾�Ʈ
        TcpClient tcpClient;//Ŭ���̾�Ʈ
        NetworkStream stream;
        NetworkStream chatStream;
        int tcpCount = 0;
        bool gameReady = false;
        bool gameReadyTo = false;
        string gameID = "";
        string gameIDTo = "";

        bool turn = false;//���� ���� true, Ÿ�� false
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
            ClientSize = new Size(2 * margin + 18 * ��size + 30 + 600, 2 * margin + 18 * ��size + 60);
            //panel1.ClientSize = new Size(2 * margin + 18 * ��size, 2 * margin + 18 * ��size);


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
                    if (�ٵ���[x, y] == STONE.white)
                        if (imageFlag == false)
                            g.FillEllipse(wBrush, margin + x * ��size - ��size / 2,
                                margin + y * ��size - ��size / 2, ��size, ��size);
                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/white.png");
                            g.DrawImage(bmp, margin + x * ��size - ��size / 2, margin + y * ��size - ��size / 2, ��size, ��size);
                        }
                    else if (�ٵ���[x, y] == STONE.black)
                        if (imageFlag == false)
                            g.FillEllipse(bBrush, margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);
                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/black.png");
                            g.DrawImage(bmp, margin + x * ��size - ��size / 2, margin + y * ��size - ��size / 2, ��size, ��size);
                        }
        }
        private void DrawBord()
        {
            g = panel1.CreateGraphics();
            //�ٵ��� �� �׸���
            for (int i = 0; i < 19; i++)
            {
                g.DrawLine(pen, new Point(margin + i * ��size, margin),
                    new Point(margin + i * ��size, margin + 18 * ��size));
                g.DrawLine(pen, new Point(margin, margin + i * ��size),
                    new Point(margin + 18 * ��size, margin + i * ��size));
            }

            //ȭ���׸���(��9�� �׸���)
            for (int x = 3; x <= 15; x += 6)
                for (int y = 3; y <= 15; y += 6)
                {
                    g.FillEllipse(bBrush,
                        margin + ��size * x - ȭ��size / 2,
                        margin + ��size * y - ȭ��size / 2,
                        ȭ��size, ȭ��size);
                }
        }
        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            plc1.SetDevice("M2", 0);//���½�ȣ �ʱ�ȭ
        }
        //���콺 Ŭ�� �����ϱ�
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (plcFlag==true) panelPLC_MouseDown(sender, e);//�̱ۿ��� PLC ����
            else
            {
                if (tcpMitiFlag == false) panelSingle_MouseDown(sender, e);  //�̱� ����� ���
                else panelMiti_MouseDown(sender, e);//��Ƽ ����� ���
            }
        }
        //m1�� ���� �Ҷ�, m0�� �����̰� �ִ��� ���� Ȯ��,m2�� �������� d1�� x, d2�� y
        //m3�� �����̶�� ���
        private void panelPLC_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (reviveFlag == true)
            {
                ReviveGame();
                return;
            }
            //e.x�� �ȼ�����, x�� �ٵ��� ��ǥ
            int x = (e.X - margin + ��size / 2) / ��size;
            int y = (e.Y - margin + ��size / 2) / ��size;
            if (x > 18 || x < 0) return;//�ٵ��� ���� Ŭ���� ���
            if (y > 18 || y < 0) return;
            if (�ٵ���[x, y] != STONE.none) return;//Ŭ���� ���� Ŭ���� ���


            // �ٵ���[x,y]�� ���� �׸��� ���� REctangle
            Rectangle r = new Rectangle(margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);
            try
            {
                if (flag == false)//����������
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//�����������
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    �ٵ���[x, y] = STONE.black;
                    plc1.SetDevice("M24", 1);//������

                }
                else//������
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
                    �ٵ���[x, y] = STONE.white;
                    plc1.SetDevice("M34", 1);//��
                }
            }
            catch (ArgumentException ex)
            {
                // ���� ó��: �Ű� ������ �߸��Ǿ��� �� ������ �ڵ� �ۼ�
                MessageBox.Show("�̹��� ���� ��ΰ� �߸��Ǿ����ϴ�.", ex.Message);
                // ���� ���� �޽��� ��� (������)
            }
            catch (Exception ex)
            {
                // ��Ÿ ���� ó��
                MessageBox.Show("����ġ ���� ������ �߻��߽��ϴ�.", ex.Message);
                // ���� ���� �޽��� ��� (������)
            }

            plc1.SetDevice("D5", x);//�� ������
            plc1.SetDevice("D2", y);
            plc1.SetDevice("M3", 1);//������!
            CkeckOmok(x, y);

        }

        private async void panelMiti_MouseDown(object sender, MouseEventArgs e)
        {
            if (gameReady == false || gameReadyTo == false)
            {
              
                MessageBox.Show("�غ� ��ư�� ��������.");
                return;
            }

            if (turn == true)
            {

                //e.x�� �ȼ�����, x�� �ٵ��� ��ǥ
                int x = (e.X - margin + ��size / 2) / ��size;
                int y = (e.Y - margin + ��size / 2) / ��size;
                if (x > 18 || x < 0) return;//�ٵ��� ���� Ŭ���� ���
                if (y > 18 || y < 0) return;
                if (�ٵ���[x, y] != STONE.none) return;//Ŭ���� ���� Ŭ���� ���
                //TimeCheck();
                // �ٵ���[x,y]�� ���� �׸��� ���� REctangle
                Rectangle r = new Rectangle(margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);

                if (tcpListenerFlag == true)//����������//����
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//�����������
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    �ٵ���[x, y] = STONE.black;
                }
                else//������//Ŭ��
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
                    �ٵ���[x, y] = STONE.white;
                }
                string xy = x + "," + y;
                byte[] buffer = Encoding.UTF8.GetBytes(xy);
                await stream.WriteAsync(buffer, 0, buffer.Length);
                turn = false;
                //������ �ޱ����
                clickTime = DateTime.Now;//�ð��ʰ�
                CkeckOmok(x, y);

            }
        }
        private async void opponentPlayer()
        {
            while (true)
            {
                //TimeCheck();
                byte[] buffer = new byte[100];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);//�б�

                byte[] receivedBytes = new byte[bytesRead];
                Array.Copy(buffer, receivedBytes, bytesRead);//�迭ũ�� ����

                string receivedString = Encoding.UTF8.GetString(receivedBytes);
                string[] xyStrings = receivedString.Split(',');

                int x = int.Parse(xyStrings[0]);
                int y = int.Parse(xyStrings[1]);
                Rectangle r = new Rectangle(margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);
                if (turn == false)
                {
                    if (tcpListenerFlag == true)//�����϶�, �� �ޱ�
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
                        �ٵ���[x, y] = STONE.white;
                    }
                    else//Ŭ���϶�, ���� �� �ޱ�
                    {
                        if (imageFlag == false) g.FillEllipse(bBrush, r);

                        else
                        {
                            Bitmap bmp = new Bitmap("../../../Images/black.png");//�����������
                            g.DrawImage(bmp, r);
                        }
                        DrawStonesSequence(stoneCnt, Brushes.White, r);
                        lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                        flag = true;
                        �ٵ���[x, y] = STONE.black;
                    }
                    turn = true;

                    CkeckOmok(x, y);
                    receiveTime = DateTime.Now; //�ð��ʰ�//�ް��������

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
                DialogResult res = MessageBox.Show($"�ð� �ʰ�\r\n {gameIDTo}���� �¸��ϼ̽��ϴ�.",
                    "��������", MessageBoxButtons.YesNo);
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
                DialogResult res = MessageBox.Show($"�ð� �ʰ�\r\n {gameID}���� �¸��ϼ̽��ϴ�.");
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
            //e.x�� �ȼ�����, x�� �ٵ��� ��ǥ
            int x = (e.X - margin + ��size / 2) / ��size;
            int y = (e.Y - margin + ��size / 2) / ��size;
            if (x > 18 || x < 0) return;//�ٵ��� ���� Ŭ���� ���
            if (y > 18 || y < 0) return;
            if (�ٵ���[x, y] != STONE.none) return;//Ŭ���� ���� Ŭ���� ���


            // �ٵ���[x,y]�� ���� �׸��� ���� REctangle
            Rectangle r = new Rectangle(margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);
            try
            {
                if (flag == false)//����������
                {
                    if (imageFlag == false) g.FillEllipse(bBrush, r);

                    else
                    {
                        Bitmap bmp = new Bitmap("../../../Images/black.png");//�����������
                        g.DrawImage(bmp, r);
                    }
                    DrawStonesSequence(stoneCnt, Brushes.White, r);
                    lstRevive.Add(new Revive(x, y, STONE.black, stoneCnt++));
                    flag = true;
                    �ٵ���[x, y] = STONE.black;
                }
                else//������
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
                    �ٵ���[x, y] = STONE.white;
                }
            }
            catch (ArgumentException ex)
            {
                // ���� ó��: �Ű� ������ �߸��Ǿ��� �� ������ �ڵ� �ۼ�
                MessageBox.Show("�̹��� ���� ��ΰ� �߸��Ǿ����ϴ�.", ex.Message);
                // ���� ���� �޽��� ��� (������)
            }
            catch (Exception ex)
            {
                // ��Ÿ ���� ó��
                MessageBox.Show("����ġ ���� ������ �߻��߽��ϴ�.", ex.Message);
                // ���� ���� �޽��� ��� (������)
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

            Rectangle r = new Rectangle(margin + ��size * x - ��size / 2, margin + ��size * y - ��size / 2, ��size, ��size);
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
                �ٵ���[x, y] = STONE.black;
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
                �ٵ���[x, y] = STONE.white;
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
            //����
            int cnt = 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++)
                if (�ٵ���[x, y] == �ٵ���[i, y]) cnt++;
                else break;
            for (int i = x - 1; i >= 0 && i <= 18; i--)
                if (�ٵ���[x, y] == �ٵ���[i, y]) cnt++;
                else break;

            //����
            int vcnt = 1;
            for (int i = y + 1; i >= 0 && i <= 18; i++)
                if (�ٵ���[x, y] == �ٵ���[x, i]) vcnt++;
                else break;
            for (int i = y - 1; i >= 0 && i <= 18; i--)
                if (�ٵ���[x, y] == �ٵ���[x, i]) vcnt++;
                else break;

            //�밢��
            int rcnt = 1;
            int yy = y + 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++, yy++)
            {
                if (yy < 0 || yy > 18) break;
                if (�ٵ���[x, y] == �ٵ���[i, yy]) rcnt++;
                else break;
            }
            yy = y - 1;
            for (int i = x - 1; i >= 0 && i <= 18; i--, yy--)
            {
                if (yy < 0 || yy > 18) break;
                if (�ٵ���[x, y] == �ٵ���[i, yy]) rcnt++;
                else break;
            }

            //���밢��
            int lcnt = 1;
            yy = y - 1;
            for (int i = x + 1; i >= 0 && i <= 18; i++, yy--)
            {
                if (yy < 0 || yy > 18) break;
                if (�ٵ���[x, y] == �ٵ���[i, yy]) lcnt++;
                else break;
            }

            yy = y + 1;
            for (int i = x - 1; i >= 0 && i <= 18; i--, yy++)
            {
                if (yy < 0 || yy > 18) break;
                if (�ٵ���[x, y] == �ٵ���[i, yy]) lcnt++;
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
            DialogResult res = MessageBox.Show(�ٵ���[x, y].ToString().ToUpper()
                + "Wins!\n���ο� ������ �����Ͻÿ�", "��������", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
                NewGame();
            else if (res == DialogResult.No)
                this.Close();

        }
        private void saveGame()
        {
            if (reviveFlag == true)
                return;//���� ��忡���� �������� ����

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
        private void �ٽý���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            InitializeOmok();
        }
        private void InitializeOmok()
        {
            ����ToolStripMenuItem.Checked = false;
            imageFlag = true;//�̹���?�׸���?
            flag = false;//false =������, true=��
            sequenceFlag = true;
            stoneCnt = 1;//����
            reviveFlag = false;//���� ������� �˸��� �÷���
            sequence = 0; //���⿡ ���Ǵ� ����
            clickTime = DateTime.Now.AddDays(7);
            receiveTime = DateTime.Now.AddDays(7);
            for (int x = 0; x < 19; x++)//�ٵ��� ��� �迭 �ʱ�ȭ
                for (int y = 0; y < 19; y++)
                    �ٵ���[x, y] = STONE.none;
            lstRevive.Clear(); //����Ʈ
            panel1.Refresh();//�ǳ� �ʱ�ȭ
            DrawBord();//�ٵ��Ǳ׸���

            if (plcFlag==true)
            {
                plc1.SetDevice("M1", 0);
                Thread.Sleep(500);
                plc1.SetDevice("M1", 1);
            }
            
        }
        private void �̹���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageFlag = true;
            �̹���ToolStripMenuItem.Checked = true;
            �׸���ToolStripMenuItem.Checked = false;
        }
        private void �׸���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageFlag = false;
            �׸���ToolStripMenuItem.Checked = true;
            �̹���ToolStripMenuItem.Checked = false;
        }
        private void ����ǥ��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sequenceFlag = true;
            ����ǥ��ToolStripMenuItem.Checked = true;
            ����ǥ�þ���ToolStripMenuItem.Checked = false;
        }
        private void ����ǥ�þ���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            sequenceFlag = false;
            ����ǥ�þ���ToolStripMenuItem.Checked = true;
            ����ǥ��ToolStripMenuItem.Checked = false;
        }
        private void ����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ����ToolStripMenuItem.Checked = true;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = dirName;
            ofd.Filter = "Omok file(*.omk)|*.omk";
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            sequenceFlag = true;

            InitializeOmok();//���� ���� ���̶�� �ʱ�ȭ
            try
            {
                StreamReader r = File.OpenText(fileName);
                string line = "";

                //���� ������ ���پ� �о lstRevive ����Ʈ�� �ִ´�.
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
            reviveFlag = true;//���� �غ�
            sequence = 0;//���� ���� �ʱ�ȭ
        }
        private void Clear�ٵ���()
        {
            for (int x = 0; x < 19; x++)
                for (int y = 0; y < 19; y++)
                    �ٵ���[x, y] = STONE.none;
        }
        private void �̱۸��ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tcpMitiFlag = false;
            �̱۸��ToolStripMenuItem.Checked = true;
            ��Ƽ���ToolStripMenuItem.Checked = false;

            stream.Close();
            chatStream.Close();
            tcpClient.Close();
            chatClient.Close();
            tcpListener.Stop();
            chatListener.Stop();
        }
        private void ��Ƽ���ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult res = MessageBox.Show("��Ƽ���� �����Ͻðڽ��ϱ�?" +
                "\r\n ���� ���� ���̴� ������ ��� �ʱ�ȭ�˴ϴ�.", "��庯��", MessageBoxButtons.YesNo);
            if (res == DialogResult.Yes)
            {
                tcpMitiFlag = true;
                ��Ƽ���ToolStripMenuItem.Checked = true;
                �̱۸��ToolStripMenuItem.Checked = false;
                panel1.MouseDown -= panel1_MouseDown;//���� �� �ٽ� �߰�
                InitializeOmok();//���� �ʱ�ȭ
            }
            else if (res == DialogResult.No)
                return;
        }
        //***********************************************************�����ϱ�***********************************************************
        //1. ���� ����
        //2. �����ϱ�
        //3. ������ �ְ� �ޱ�
        // ������ ���� 


        //���� ������ ���
        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            tcpListenerFlag = true;//������ ǥ��
            ��������ToolStripMenuItem.Checked = true;
        }
        //���� �� �غ� ��ȣ ���� ������ ���

        //���� ��ư�� ������
        private async void btnConnect_Click(object sender, EventArgs e)
        {

            if (tcpMitiFlag == false) return;//��Ƽ��尡 �ƴҰ�� ��Ȱ��ȭ
                                             //ip,�����κ�
                                             //ip���� �� ���� ��ȣ ������
            if (tcpListenerFlag == true)
            {
                tcpListener = new TcpListener(IPAddress.Parse(tbIP.Text), 50000);//������ ���
                tcpListener.Start();
                tcpClient = await tcpListener.AcceptTcpClientAsync();
                idExchange();//���̵� ��ȯ

                chatListener = new TcpListener(IPAddress.Parse(tbIP.Text), 50001);
                chatListener.Start();
                chatClient = await chatListener.AcceptTcpClientAsync();
            }
            else
            {
                tcpClient = new TcpClient();//Ŭ���̾�Ʈ�ϰ��
                await tcpClient.ConnectAsync(tbIP.Text, 50000);//Ŭ���̾�Ʈ�� ���
                idExchange();//���̵� ��ȯ

                chatClient = new TcpClient();
                await chatClient.ConnectAsync(tbIP.Text, 50001);

            }
            chatStream = chatClient.GetStream();
            chating();




        }
        private async void idExchange()//���̵� ��ȯ
        {
            bool connectSuccess = false;
            stream = tcpClient.GetStream();
            if (tcpListenerFlag == true)
            {
                //���̵� ����
                byte[] buffer = new byte[256];
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                //���ڵ� �� �ֱ�
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                gameIDTo = message;
                gameID = tbID.Text;
                lbChat.Items.Add(message + "���� �����ϼ̽��ϴ�.");

                //���̵� ������
                buffer = new byte[256];
                string text = tbID.Text;
                read = text.Length;
                buffer = BitConverter.GetBytes(read);
                stream.Write(Encoding.UTF8.GetBytes(text));
                connectSuccess = true;
            }
            else
            {
                //���̵� ����
                byte[] buffer = new byte[256];
                string text = tbID.Text;
                int read = text.Length;
                buffer = BitConverter.GetBytes(read);
                stream.Write(Encoding.UTF8.GetBytes(text));

                //���̵� ����
                buffer = new byte[256];
                read = await stream.ReadAsync(buffer, 0, buffer.Length);
                //���ڵ� �� �ֱ�
                string message = Encoding.UTF8.GetString(buffer, 0, read);
                lbChat.Items.Add(message + "�� �濡 �����ϼ̽��ϴ�.");
                gameIDTo = message;
                gameID = tbID.Text;
                connectSuccess = true;

            }
            if (connectSuccess == true)
                panel1.MouseDown += panel1_MouseDown;//���� ������ �ٽ� ����  
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
                lbChat.Items.Add($"{gameIDTo}���� �غ� �Ϸ��ϼ̽��ϴ�.");
                turn = true;
            }
            else
            {
                if (gameReady == true) writeBuffer[0] = 1;
                await stream.WriteAsync(writeBuffer, 0, writeBuffer.Length);

                _ = await stream.ReadAsync(readBuffer, 0, readBuffer.Length);
                if (readBuffer[0] == 1) gameReadyTo = true;
                lbChat.Items.Add($"{gameIDTo}���� �غ� �Ϸ��ϼ̽��ϴ�.");
                turn = false;
            }
            opponentPlayer();//���� ���϶� �� �ޱ⸦ ����ϱ�


        }
        private async void chating()
        {

            while (true)//�о ���� �κ�
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

        private void pLC����ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pLC����ToolStripMenuItem.Checked = true;
            pLC��������ToolStripMenuItem.Checked = false;
            plcFlag = true;
            plc1 = new ActEasyIF();
            plc1.ActLogicalStationNumber = 1;
            int conErr = plc1.Open();

            if (conErr == 0)
            {
                lbChat.Items.Add("PLC�� �����ϼ̽��ϴ�.");
                tmrUpdate.Start();
                plc1.SetDevice("M1", 1);
                plc1.SetDevice("M24", 0);
                plc1.SetDevice("M34", 0);

            }
            else
            {
                lbChat.Items.Add("���� ����" + conErr);
            }
        }

        private void pLC��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pLC��������ToolStripMenuItem.Checked = true;
            pLC����ToolStripMenuItem.Checked = false;
            plcFlag = false;

            plc1.SetDevice("M1", 0);
            plc1.Close();
            lbChat.Items.Add("������ �����Ǿ����ϴ�.");
            tmrUpdate.Stop();
        }

        private void ��������ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plc1.SetDevice("M2", 1);
        }
    }
}

