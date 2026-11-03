using System.Drawing;

namespace Projeto_4_bim
{
    public partial class Form1 : Form
    {
        // Caminhos das imagens de entrada
        string caminhoPaisagem = @"c:\Imagens\Im.jpg";
        string caminhoHomem = @"c:\Imagens\homem.jpg";
        string caminhoBalao = @"c:\Imagens\Balao.jpg";
        string caminhoAviao = @"c:\Imagens\Aviao2.jpg";

        // Caminhos das imagens de saida
        string caminhoProcessada = @"c:\Imagens\Processada.jpg";
        string caminhoProcessadaBrilho = @"c:\Imagens\ProcessadaBrilho.jpg";
        string caminhoProcessadaBinaria = @"c:\Imagens\ProcessadaBinaria.jpg";
        string caminhoProcessadaCinza = @"c:\Imagens\ProcessadaCinza.jpg";
        string caminhoProcessadaCinzaRotacionada = @"c:\Imagens\ProcessadaCinzaRotacionada.jpg";

        // Limiares para remover o fundo de cada imagem
        int limiarHomem = 12;
        int limiarBalao = 32;
        int limiarAviao = 45;

        // Contador para alternar a imagem rotacionada no botao
        int contadorRotacao = 0;

        public Form1()
        {
            InitializeComponent();
        }

        // Enunciado item (c): botao unico que realiza todo o processamento.
        public void btnProcessar_Click(object sender, EventArgs e)
        {
            // Uma unica primitiva cola as 3 imagens e salva a imagem final
            ColarImagens(
                caminhoPaisagem,
                caminhoHomem, limiarHomem,
                caminhoBalao, limiarBalao,
                caminhoAviao, limiarAviao,
                caminhoProcessada);

            // Aplica as demais primitivas sobre a imagem final salva
            ImgGrayLevel(caminhoProcessada, caminhoProcessadaCinza);                  // item (i)
            Threshold(caminhoProcessadaCinza, caminhoProcessadaBinaria, 126);         // item (j): T = 126
            Iluminar(caminhoProcessada, caminhoProcessadaBrilho, 85, 85, 85);         // item (h): brilho 85
            TransporImagem(caminhoProcessadaCinza, caminhoProcessadaCinzaRotacionada); // item (n)

            // Mostra as 4 imagens na tela usando .Load
            picOriginal.Load(caminhoProcessada);
            picBrilho.Load(caminhoProcessadaBrilho);
            picCinza.Load(caminhoProcessadaCinza);
            picRotacao.Load(caminhoProcessadaCinza);

            contadorRotacao = 0;
        }

        // Botao simples que alterna entre a imagem cinza e a rotacionada.
        public void btnRotacionar_Click(object sender, EventArgs e)
        {
            contadorRotacao++;
            bool mostrarRotacionada = contadorRotacao % 2 == 1;

            if (mostrarRotacionada)
                picRotacao.Load(caminhoProcessadaCinzaRotacionada);
            else
                picRotacao.Load(caminhoProcessadaCinza);
        }

        // EVIDENCIA (RA7 p.16): cor = Color.FromArgb(r, g, b);
        public static Color CriarCor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        // Verifica se um pixel esta dentro do limiar da cor de fundo.
        public static bool PixelEhFundo(Color pixel, Color corFundo, int limiar)
        {
            return corFundo.R - limiar < pixel.R && corFundo.R + limiar > pixel.R &&
                   corFundo.G - limiar < pixel.G && corFundo.G + limiar > pixel.G &&
                   corFundo.B - limiar < pixel.B && corFundo.B + limiar > pixel.B;
        }

        // UNICA primitiva que cola as 3 imagens na paisagem e salva a imagem final.
        // Nao recebe Bitmap por parametro; so caminhos.
        public static void ColarImagens(string caminhoFundo,
                                        string caminhoHomem, int limiarHomem,
                                        string caminhoBalao, int limiarBalao,
                                        string caminhoAviao, int limiarAviao,
                                        string caminhoSaida)
        {
            Bitmap fundo = new Bitmap(caminhoFundo);

            // Cola o homem na posicao (300, 500)
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

            // Cola o balao na posicao (500, 0)
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

            // Cola o aviao na posicao (200, 0)
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

        // I'(x,y) = I(x,y) + deltaB em cada canal, com clamp min(max(v,0),255).
        // EVIDENCIA (RA7 p.27 e p.29).
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

        // GrayLevel: g = (r*0.30) + (g*0.59) + (b*0.11) (RA7 p.21).
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

        // Thresholding: se I(i,j) <= T entao 0, senao 255 (RA7 p.23).
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

        // Rotacao por transposicao de matriz: I'(x,y) = I(y,x) (RA6 p.20).
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
