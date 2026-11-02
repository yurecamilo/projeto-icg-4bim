using System.Drawing;

namespace Projeto_4_bim
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Enunciado item (c): um unico botao faz todo o processamento.
        public void btnProcessar_Click(object sender, EventArgs e)
        {
            // EVIDENCIA (RA7 p.14): Bitmap img = new Bitmap("caminho")
            Bitmap a = new Bitmap(@"c:\Imagens\Im.jpg");

            // Enunciado item (a): filtro nas intensidades do fundo de B, C e D.
            // A cor do fundo e obtida de GetPixel(0,0), como no codigo do colega.
            Bitmap b = FiltrarFundo(new Bitmap(@"c:\Imagens\Aviao2.jpg"));
            Bitmap c = FiltrarFundo(new Bitmap(@"c:\Imagens\Balao.jpg"));
            Bitmap d = FiltrarFundo(new Bitmap(@"c:\Imagens\homem.jpg"));

            // Posicoes conforme o codigo do colega (aviao 200,0; balao 500,0; homem 300,500).
            // Tamanho original de cada imagem (sem escala).
            b = SobreCamada(b, a.Width, a.Height, 200, 0);
            c = SobreCamada(c, a.Width, a.Height, 500, 0);
            d = SobreCamada(d, a.Width, a.Height, 300, 500);

            // Enunciado itens 1 e (k): A + B + C + D = E numa unica funcao.
            Bitmap E = JuntarImagens(a, b, c, d);

            // Enunciado itens (m), (e) e (o): quatro saidas .jpg em c:\Imagens.
            E.Save(@"c:\Imagens\E_original.jpg");
            Bitmap brilho = Iluminar(E, 85);     // item (h): brilho 85
            brilho.Save(@"c:\Imagens\E_brilho85.jpg");
            Bitmap cinza = ImgGrayLevel(E);       // item (i)
            cinza.Save(@"c:\Imagens\E_cinza.jpg");
            Bitmap binaria = Threshold(cinza, 126); // item (j): T = 126
            binaria.Save(@"c:\Imagens\E_binaria.jpg");
            Rotacionar(cinza).Save(@"c:\Imagens\E_cinza_rotacionada.jpg"); // item (n)

            // Mostra as 4 imagens principais na tela (item g: opcional, mas agora ativo).
            pictureBox1.Image = E;
            pictureBox2.Image = brilho;
            pictureBox3.Image = cinza;
            pictureBox4.Image = binaria;
        }

        // EVIDENCIA (RA7 p.16): cor = Color.FromArgb(r, g, b);
        public static Color IniciaCor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        // I'(x,y) = I(x,y) + deltaB com clamp min(max(v,0),255) (RA7 p.27 e p.29).
        public static Bitmap Iluminar(Bitmap img, int deltaB)
        {
            Bitmap imgnova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int r = img.GetPixel(i, j).R;
                    int g = img.GetPixel(i, j).G;
                    int b = img.GetPixel(i, j).B;
                    imgnova.SetPixel(i, j, IniciaCor(
                        Math.Max(0, Math.Min(255, r + deltaB)),
                        Math.Max(0, Math.Min(255, g + deltaB)),
                        Math.Max(0, Math.Min(255, b + deltaB))));
                }
            }
            return imgnova;
        }

        // g = (r*0.30) + (g*0.59) + (b*0.11) (RA7 p.21).
        public static Bitmap ImgGrayLevel(Bitmap img)
        {
            Bitmap imgnova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    float r = img.GetPixel(i, j).R;
                    float g = img.GetPixel(i, j).G;
                    float b = img.GetPixel(i, j).B;
                    int rgb = (int)(r * 0.3 + g * 0.59 + b * 0.11);
                    imgnova.SetPixel(i, j, IniciaCor(rgb, rgb, rgb));
                }
            }
            return imgnova;
        }

        // Se I(i,j) <= T entao 0, senao 255 (RA7 p.23).
        public static Bitmap Threshold(Bitmap img, int t)
        {
            Bitmap imgnova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    float r = img.GetPixel(i, j).R;
                    float g = img.GetPixel(i, j).G;
                    float b = img.GetPixel(i, j).B;
                    int rgb = (int)(r * 0.3 + g * 0.59 + b * 0.11);
                    if (rgb <= t)
                        imgnova.SetPixel(i, j, IniciaCor(0, 0, 0));
                    else
                        imgnova.SetPixel(i, j, IniciaCor(255, 255, 255));
                }
            }
            return imgnova;
        }

        // Filtra o fundo: pega a cor do pixel (0,0) e apaga os pixels com a mesma
        // cor (com tolerancia 30), mantendo o objeto. Mesmo if/else do slide RA7 p.18.
        public static Bitmap FiltrarFundo(Bitmap img)
        {
            Color fundo = img.GetPixel(0, 0);
            Bitmap imgnova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int r = img.GetPixel(i, j).R;
                    int g = img.GetPixel(i, j).G;
                    int b = img.GetPixel(i, j).B;
                    if (Math.Abs(r - fundo.R) < 30 && Math.Abs(g - fundo.G) < 30 && Math.Abs(b - fundo.B) < 30)
                        imgnova.SetPixel(i, j, IniciaCor(0, 0, 0));
                    else
                        imgnova.SetPixel(i, j, IniciaCor(r, g, b));
                }
            }
            return imgnova;
        }

        // Juncao A + B + C + D = E numa unica funcao (item k): fundo preto (0,0,0)
        // nao contribui, objeto (cor != preta) entra na soma (RA7 p.15 e p.16).
        public static Bitmap JuntarImagens(Bitmap a, Bitmap b, Bitmap c, Bitmap d)
        {
            Bitmap imgnova = new Bitmap(a.Width, a.Height);
            for (int i = 0; i <= a.Width - 1; i++)
            {
                for (int j = 0; j <= a.Height - 1; j++)
                {
                    Color cor = a.GetPixel(i, j);
                    Color pb = b.GetPixel(i, j);
                    if (pb.R != 0 || pb.G != 0 || pb.B != 0)
                        cor = pb;
                    Color pc = c.GetPixel(i, j);
                    if (pc.R != 0 || pc.G != 0 || pc.B != 0)
                        cor = pc;
                    Color pd = d.GetPixel(i, j);
                    if (pd.R != 0 || pd.G != 0 || pd.B != 0)
                        cor = pd;
                    imgnova.SetPixel(i, j, cor);
                }
            }
            return imgnova;
        }

        // Rotacao por transposicao de matriz: I'(x,y) = I(y,x) (RA6 p.20).
        public static Bitmap Rotacionar(Bitmap img)
        {
            Bitmap imgnova = new Bitmap(img.Height, img.Width);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    imgnova.SetPixel(j, i, img.GetPixel(i, j));
                }
            }
            return imgnova;
        }

        // Copia a imagem para dentro de um Bitmap do tamanho de A na posicao
        // x' = x + tx, y' = y + ty (RA6 p.12). Fundo preto (0,0,0) nao contribui.
        public static Bitmap SobreCamada(Bitmap img, int coluna, int linha, int tx, int ty)
        {
            Bitmap imgnova = new Bitmap(coluna, linha);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int x = i + tx;
                    int y = j + ty;
                    if (x >= 0 && x <= coluna - 1 && y >= 0 && y <= linha - 1)
                        imgnova.SetPixel(x, y, img.GetPixel(i, j));
                }
            }
            return imgnova;
        }
    }
}
