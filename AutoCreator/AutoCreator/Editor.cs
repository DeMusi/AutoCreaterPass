using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoCreator
{
    public partial class Editor : Form
    {
        Rectangle selrect; //выделенный прямоугольник
        Point orig; //начальные координаты прямоугольника
        Pen pen = new Pen(Brushes.White, 0.8f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };//цвет и стиль выделения
        public Bitmap img, img1, OrigImg;
        public SaveFileDialog svf;
        Size S;
        int W, H;
        double W1, H1;
        double FK, FK1;
        public Editor()
        {
            InitializeComponent();
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
        }
       
        private void Editor_Layout(object sender, LayoutEventArgs e)
        {           
            Form1 main = this.Owner as Form1; //Определяем предка текущей формы
            OrigImg = (Bitmap)main.pictureBox1.Image.Clone(); //Берем фото из главной формы и назначаем его как оригинал 
            S = FKoff(OrigImg.Height, OrigImg.Width); 
            this.pictureBox1.Image = OrigImg;//показываю фотку на вторую форму при ее запуске
            img = (Bitmap)this.pictureBox1.Image;
            img = new Bitmap(img, S); //масштабирую фото под размеры пикче бокса эдитора
            this.pictureBox1.Image = img;
            OrigImg = (Bitmap)main.pictureBox1.Image.Clone();       
        }

        public Size FKoff(int He, int Wi) // Расчет коэффицента масштабирования для окна эдитора
        {
            if (He != 0 && Wi != 0) //находим масштабный коэффициент
            {
                W1 = Wi;
                H1 = He;
                FK = 525 / W1;
                FK1 = 700 / H1;
                FK = Math.Min(FK, FK1);
            }
            W = Convert.ToInt32(Wi * FK); H = Convert.ToInt32(He * FK);
            S = new Size(W, H);
            return S;
        }

        public Size MKoff(int He, int Wi) // Расчет коэффицента масштабирования для главного окна
        {
            if (He != 0 && Wi != 0) //находим масштабный коэффициент
            {
                W1 = Wi;
                H1 = He;
                FK = 189 / W1;
                FK1 = 252 / H1;
                FK = Math.Min(FK, FK1);
            }
            W = Convert.ToInt32(Wi * FK); H = Convert.ToInt32(He * FK);
            S = new Size(W, H);
            return S;
        }
       
        public Size OrigKoff(int He, int Wi) // Расчет коэффицента масштабирования для 1024x768
        {
            if (He != 0 && Wi != 0) //находим масштабный коэффициент
            {
                W1 = Wi;
                H1 = He;
                FK = 768 / W1;
                FK1 = 1024 / H1;
                FK = Math.Min(FK, FK1);
            }
            W = Convert.ToInt32(Wi * FK); H = Convert.ToInt32(He * FK1);
            S = new Size(W, H);
            return S;
        }

        //Обрезка
        private void button3_Click(object sender, EventArgs e)
        {
            if (selrect.X + selrect.Width > this.pictureBox1.Image.Width || selrect.Y + selrect.Height > this.pictureBox1.Image.Height || selrect.X < 0 || selrect.Y < 0)
                MessageBox.Show("Вы вышли за пределы редактируемой формы!", "Ошибка!", MessageBoxButtons.OK);
           else
           if (selrect.Width != 0 && selrect.Height != 0)
            {
                Brush cBrush = new Pen(Color.FromArgb(150, Color.White)).Brush;
                Form1 main = this.Owner as Form1; //Определяем предка текущей формы
                Bitmap PrevCropArea = (Bitmap)this.pictureBox1.Image;//оригинал
                Bitmap temp = (Bitmap)PrevCropArea.Clone(); //Темп картинка, с ним делаем что хотим, затем заменяем им оригинал
                Graphics myGraphics = Graphics.FromImage(temp);
                 //прямоугольники для сокрытия обрезанных участков
                 Rectangle rect1 = new Rectangle(0, 0, this.pictureBox1.Width, selrect.Y);
                 Rectangle rect2 = new Rectangle(0, selrect.Y, selrect.X, selrect.Height);
                 Rectangle rect3 = new Rectangle(0, selrect.Y + selrect.Height, this.pictureBox1.Width, this.pictureBox1.Height - selrect.Y - selrect.Height);
                 Rectangle rect4 = new Rectangle(selrect.X + selrect.Width, selrect.Y, this.pictureBox1.Width - selrect.X - selrect.Width, selrect.Height);
                 myGraphics.FillRectangle(cBrush, rect1);
                 myGraphics.FillRectangle(cBrush, rect2);
                 myGraphics.FillRectangle(cBrush, rect3);
                 myGraphics.FillRectangle(cBrush, rect4);
                 this.pictureBox1.Image = (Bitmap)temp.Clone();
             
                DialogResult dialogResult = MessageBox.Show("Уверены что хотите обрезать эту часть?", "Предупреждение!", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    img = (Bitmap)this.pictureBox1.Image;//current
                    img1 = (Bitmap)img.Clone();//temp
                    if (selrect.X + selrect.Width > img.Width)
                        selrect.Width = img.Width - selrect.X;
                    if (selrect.Y + selrect.Height > img.Height)
                        selrect.Height = img.Height - selrect.Y;

                    FK1 = Convert.ToDouble(OrigImg.Height) / Convert.ToDouble(S.Height);
                    FK = Convert.ToDouble(OrigImg.Width) / Convert.ToDouble(S.Width);

                    int X = Convert.ToInt32(selrect.X * FK1), Y = Convert.ToInt32(selrect.Y * FK), Wi = Convert.ToInt32(selrect.Width * FK1), He = Convert.ToInt32(selrect.Height * FK);
                    Rectangle rectX = new Rectangle(X, Y, Wi, He);
                    
                    //тут надо что-то придумать: Почему переполняется память? Подозрения на OrigImg 
                    img = (Bitmap)OrigImg.Clone(rectX, OrigImg.PixelFormat);
                    
                    Size OrS = OrigKoff(He, Wi);
                    S = FKoff(He, Wi);
                    img = new Bitmap(img, S);
                    this.pictureBox1.Image = img; //показываю то что получилось в окне редактора
                   
                    img1 = new Bitmap(img, OrS);
                    selrect.Width = 0; selrect.Height = 0; // тут я убрал баг с оставшимся прямоугольником после обрезки
                    
                    // Сохранение может решить мои трабл
                    svf = new SaveFileDialog();
                    svf.Filter = "JPEG|*.jpg";
                    svf.Title = "Сохранить как";
                    if (main.textBox2.TextLength != 0 && main.textBox3.TextLength != 0 && main.textBox4.TextLength != 0)
                        svf.FileName = main.textBox2.Text + " " + main.textBox3.Text + " " + main.textBox4.Text;//если были назначены ФИО то передаем их как имя
                    else svf.FileName = main.textBox1.Text; //иначе передаем в название табельный номер
                    if (main.textBox2.TextLength != 0 && main.textBox3.TextLength != 0 && main.textBox4.TextLength != 0 && main.textBox1.TextLength != 0)
                        svf.FileName = main.textBox1.Text;//если введены все поля, то выводим в название только табельный
                    if (svf.ShowDialog() == DialogResult.OK)
                    {
                        img1.Save(svf.FileName);
                        Size MP = MKoff(img1.Height, img1.Width);
                        img1 = new Bitmap(img1, MP);

                        main.pictureBox1.ImageLocation = svf.FileName;
                        main.pictureBox1.Image = img1;
                    }
                    else
                        this.pictureBox1.Image = (Bitmap)PrevCropArea;//если "Отмена" загружаем предыдущую фотку

                }
                else if (dialogResult == DialogResult.No)
                {
                    this.pictureBox1.Image = (Bitmap)PrevCropArea;//если "Нет" загружаем предыдущую фотку
                }
            }
            else MessageBox.Show("Вы не выделили область.", "Ошибка!", MessageBoxButtons.OK);
           this.button4.Enabled = true;
        }

        //Рисование мышкой с нажатой кнопкой
        private void Selection_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen, selrect);
        }
        
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //Назначаем процедуру рисования при выделении
            pictureBox1.Paint -= pictureBox1_Paint;
            pictureBox1.Paint += Selection_Paint;
            orig = e.Location;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //Возвращаем основную процедуру рисования
            pictureBox1.Paint -= Selection_Paint;
            pictureBox1.Paint += pictureBox1_Paint;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //при движении мышкой с зажатой лкм считаем прямоугольник и обновляем picturebox
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                selrect = GetSelRectangle(orig, e.Location);
           if (e.Button == System.Windows.Forms.MouseButtons.Left)
                (sender as PictureBox).Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(Pens.Black, selrect);//цвет рамки
        }

        //возвращает параметры выделенной рамки
        Rectangle GetSelRectangle(Point orig, Point location)//нужно вычислять во сколько раз изменена фотка и домножить параметры на это число
        {
            int deltaX = location.X - orig.X, deltaY = location.Y - orig.Y;
            double K = 1.3333333, K1 = 0.75;
            Size s;
            if (deltaY != 0)
            {
                if ((Math.Abs(deltaX) / Math.Abs(deltaY)) == K || (Math.Abs(deltaX) / Math.Abs(deltaY)) == K1)//критерий пропорциональности
                {
                   s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
                }
                else
                {
                    if (Math.Abs(deltaX) < Math.Abs(deltaY))//ширина меньше чем высота
                    {
                        deltaY = (int)((deltaX / K1));
                        deltaX = (int)((deltaY * K1));
                        s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
                    }
                    if (Math.Abs(deltaX) > Math.Abs(deltaY))//ширина больше чем высота
                    {
                        deltaX = (int)((deltaY / K));
                        deltaY = (int)((deltaX * K));
                        s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
                    }
                    else { deltaX = (int)((deltaY / K1)); deltaY = (int)(deltaX * K1); s = new Size(Math.Abs(deltaX), Math.Abs(deltaY)); }
                }
            }
            else s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
            Rectangle rect = new Rectangle();
            if (deltaX >= 0 & deltaY >= 0)
                rect = new Rectangle(orig, s);
            if (deltaX < 0 & deltaY > 0)
                rect = new Rectangle(location.X, orig.Y, s.Width, s.Height);
            if (deltaX < 0 & deltaY < 0)
                rect = new Rectangle(location, s);
            if (deltaX > 0 & deltaY < 0)
                rect = new Rectangle(orig.X, location.Y, s.Width, s.Height);
            return rect;
        }

        //Сохранить как
        private void button4_Click(object sender, EventArgs e)
        {
            //Form1 main = this.Owner as Form1; //Определяем предка текущей формы
            //svf = new SaveFileDialog();
            //svf.Filter = "JPEG|*.jpg";
            //svf.Title = "Сохранить как";
            //if (main.textBox2.TextLength != 0 && main.textBox3.TextLength != 0 && main.textBox4.TextLength != 0)
            //    svf.FileName = main.textBox2.Text + " " + main.textBox3.Text + " " + main.textBox4.Text;//если были назначены ФИО то передаем их как имя
            //else svf.FileName = main.textBox1.Text; //иначе передаем в название табельный номер
            //if (main.textBox2.TextLength != 0 && main.textBox3.TextLength != 0 && main.textBox4.TextLength != 0 && main.textBox1.TextLength != 0)
            //    svf.FileName = main.textBox1.Text;//если введены все поля, то выводим в название только табельный
            //if (svf.ShowDialog() == DialogResult.OK)
            //{
            //    img1.Save(svf.FileName);
            //    Size MP = MKoff(img1.Height, img1.Width);
            //    img1 = new Bitmap(img1, MP);
                
            //    main.pictureBox1.ImageLocation = svf.FileName; 
            //    main.pictureBox1.Image = img1;
            //}
         }
    
    }
}
