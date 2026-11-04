using System.Drawing;

namespace Projeto_4_bim
{
    public partial class Form1 : Form
    {
        string caminhoPaisagem = @"c:\Imagens\Im.jpg";
        string caminhoHomem = @"c:\Imagens\homem.jpg";
        string caminhoBalao = @"c:\Imagens\Balao.jpg";
        string caminhoAviao = @"c:\Imagens\Aviao2.jpg";

        string caminhoProcessada = @"c:\Imagens\Processada.jpg";
        string caminhoProcessadaBrilho = @"c:\Imagens\ProcessadaBrilho.jpg";
        string caminhoProcessadaBinaria = @"c:\Imagens\ProcessadaBinaria.jpg";
        string caminhoProcessadaCinza = @"c:\Imagens\ProcessadaCinza.jpg";
        string caminhoProcessadaCinzaRotacionada = @"c:\Imagens\ProcessadaCinzaRotacionada.jpg";

        int limiarHomem = 12;
        int limiarBalao = 32;
        int limiarAviao = 45;

        int contadorRotacao = 0;

        public Form1()
        {
            InitializeComponent();
        }

        public void btnProcessar_Click(object sender, EventArgs e)
        {
            ColarImagens(
                caminhoPaisagem,
                caminhoHomem, limiarHomem,
                caminhoBalao, limiarBalao,
                caminhoAviao, limiarAviao,
                caminhoProcessada);

            ImgGrayLevel(caminhoProcessada, caminhoProcessadaCinza);
            Threshold(caminhoProcessadaCinza, caminhoProcessadaBinaria, 126);
            Iluminar(caminhoProcessada, caminhoProcessadaBrilho, 85, 85, 85);
            TransporImagem(caminhoProcessadaCinza, caminhoProcessadaCinzaRotacionada);

            picOriginal.Load(caminhoProcessada);
            picBrilho.Load(caminhoProcessadaBrilho);
            picCinza.Load(caminhoProcessadaCinza);
            picRotacao.Load(caminhoProcessadaCinza);

            contadorRotacao = 0;
        }

        public void btnRotacionar_Click(object sender, EventArgs e)
        {
            contadorRotacao++;
            bool mostrarRotacionada = contadorRotacao % 2 == 1;

            if (mostrarRotacionada)
                picRotacao.Load(caminhoProcessadaCinzaRotacionada);
            else
                picRotacao.Load(caminhoProcessadaCinza);
        }

        public static Color CriarCor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        public static bool PixelEhFundo(Color pixel, Color corFundo, int limiar)
        {
            return corFundo.R - limiar < pixel.R && corFundo.R + limiar > pixel.R &&
                   corFundo.G - limiar < pixel.G && corFundo.G + limiar > pixel.G &&
                   corFundo.B - limiar < pixel.B && corFundo.B + limiar > pixel.B;
        }

        public static void ColarImagens(string caminhoFundo,
                                        string caminhoHomem, int limiarHomem,
                                        string caminhoBalao, int limiarBalao,
                                        string caminhoAviao, int limiarAviao,
                                        string caminhoSaida)
        {
            Bitmap fundo = new Bitmap(caminhoFundo);

            using (Bitmap homem = new Bitmap(caminhoHomem))
            {
                Color corFundo = homem.GetPixel(0, 0);
                for (int i = 0; i <= homem.Width - 1; i++)
                {
                    for (int j = 0; j <= homem.Height - 1; j++)
                    {
                        int x = 300 + i;
                        int y = 500 + j;
                        if (x <= fundo.Width - 1 && y <= fundo.Height - 1)
                        {
                            Color pixel = homem.GetPixel(i, j);
                            if (PixelEhFundo(pixel, corFundo, limiarHomem) == false)
                                fundo.SetPixel(x, y, pixel);
                        }
                    }
                }
            }

            using (Bitmap balao = new Bitmap(caminhoBalao))
            {
                Color corFundo = balao.GetPixel(0, 0);
                for (int i = 0; i <= balao.Width - 1; i++)
                {
                    for (int j = 0; j <= balao.Height - 1; j++)
                    {
                        int x = 500 + i;
                        int y = 0 + j;
                        if (x <= fundo.Width - 1 && y <= fundo.Height - 1)
                        {
                            Color pixel = balao.GetPixel(i, j);
                            if (PixelEhFundo(pixel, corFundo, limiarBalao) == false)
                                fundo.SetPixel(x, y, pixel);
                        }
                    }
                }
            }

            using (Bitmap aviao = new Bitmap(caminhoAviao))
            {
                Color corFundo = aviao.GetPixel(0, 0);
                for (int i = 0; i <= aviao.Width - 1; i++)
                {
                    for (int j = 0; j <= aviao.Height - 1; j++)
                    {
                        int x = 200 + i;
                        int y = 0 + j;
                        if (x <= fundo.Width - 1 && y <= fundo.Height - 1)
                        {
                            Color pixel = aviao.GetPixel(i, j);
                            if (PixelEhFundo(pixel, corFundo, limiarAviao) == false)
                                fundo.SetPixel(x, y, pixel);
                        }
                    }
                }
            }

            fundo.Save(caminhoSaida);
        }

        public static void Iluminar(string caminhoEntrada, string caminhoSaida, int brilhoR, int brilhoG, int brilhoB)
        {
            Bitmap img = new Bitmap(caminhoEntrada);
            Bitmap imagemNova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int r = img.GetPixel(i, j).R;
                    int g = img.GetPixel(i, j).G;
                    int b = img.GetPixel(i, j).B;
                    imagemNova.SetPixel(i, j, CriarCor(
                        Math.Max(0, Math.Min(255, r + brilhoR)),
                        Math.Max(0, Math.Min(255, g + brilhoG)),
                        Math.Max(0, Math.Min(255, b + brilhoB))));
                }
            }
            imagemNova.Save(caminhoSaida);
        }

        public static void ImgGrayLevel(string caminhoEntrada, string caminhoSaida)
        {
            Bitmap img = new Bitmap(caminhoEntrada);
            Bitmap imagemNova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    float r = img.GetPixel(i, j).R;
                    float g = img.GetPixel(i, j).G;
                    float b = img.GetPixel(i, j).B;
                    int cinza = (int)(r * 0.3 + g * 0.59 + b * 0.11);
                    imagemNova.SetPixel(i, j, CriarCor(cinza, cinza, cinza));
                }
            }
            imagemNova.Save(caminhoSaida);
        }

        public static void Threshold(string caminhoEntrada, string caminhoSaida, int t)
        {
            Bitmap img = new Bitmap(caminhoEntrada);
            Bitmap imagemNova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    float r = img.GetPixel(i, j).R;
                    float g = img.GetPixel(i, j).G;
                    float b = img.GetPixel(i, j).B;
                    int cinza = (int)(r * 0.3 + g * 0.59 + b * 0.11);
                    if (cinza <= t)
                        imagemNova.SetPixel(i, j, CriarCor(0, 0, 0));
                    else
                        imagemNova.SetPixel(i, j, CriarCor(255, 255, 255));
                }
            }
            imagemNova.Save(caminhoSaida);
        }

        public static void TransporImagem(string caminhoEntrada, string caminhoSaida)
        {
            Bitmap img = new Bitmap(caminhoEntrada);
            Bitmap imagemNova = new Bitmap(img.Height, img.Width);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    imagemNova.SetPixel(j, i, img.GetPixel(i, j));
                }
            }
            imagemNova.Save(caminhoSaida);
        }
    }
}
