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
        string caminhoProcessadaHomem = @"c:\Imagens\ProcessadaHomem.jpg";
        string caminhoProcessadaHomemBalao = @"c:\Imagens\ProcessadaHomemBalao.jpg";
        string caminhoProcessada = @"c:\Imagens\Processada.jpg";
        string caminhoProcessadaBrilho = @"c:\Imagens\ProcessadaBrilho.jpg";
        string caminhoProcessadaBinaria = @"c:\Imagens\ProcessadaBinaria.jpg";
        string caminhoProcessadaCinza = @"c:\Imagens\ProcessadaCinza.jpg";
        string caminhoProcessadaCinzaRotacionada = @"c:\Imagens\ProcessadaCinzaRotacionada.jpg";

        // Limiares para remover o fundo de cada imagem (ideia do codigo do colega)
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
            // Carrega a paisagem e os objetos
            Bitmap paisagem = new Bitmap(caminhoPaisagem);
            Bitmap homem = new Bitmap(caminhoHomem);
            Bitmap balao = new Bitmap(caminhoBalao);
            Bitmap aviao = new Bitmap(caminhoAviao);

            // Cola gradualmente, salvando cada juncao intermediaria
            ColarImagem(paisagem, homem, 300, 500, limiarHomem);
            paisagem.Save(caminhoProcessadaHomem);

            Bitmap comHomem = new Bitmap(paisagem);
            ColarImagem(comHomem, balao, 500, 0, limiarBalao);
            comHomem.Save(caminhoProcessadaHomemBalao);

            Bitmap comHomemBalao = new Bitmap(comHomem);
            ColarImagem(comHomemBalao, aviao, 200, 0, limiarAviao);
            comHomemBalao.Save(caminhoProcessada);

            // Aplica as primitivas e salva cada saida
            Bitmap imagemCinza = ImgGrayLevel(comHomemBalao);
            imagemCinza.Save(caminhoProcessadaCinza);                 // item (i)

            Bitmap imagemBinaria = Threshold(imagemCinza, 126);
            imagemBinaria.Save(caminhoProcessadaBinaria);             // item (j): T = 126

            Bitmap imagemBrilho = Iluminar(comHomemBalao, 85, 85, 85);
            imagemBrilho.Save(caminhoProcessadaBrilho);               // item (h): brilho 85

            Bitmap imagemRotacionada = TransporImagem(imagemCinza);
            imagemRotacionada.Save(caminhoProcessadaCinzaRotacionada); // item (n)

            // Mostra as 4 imagens na tela
            picOriginal.Image = new Bitmap(caminhoProcessada);
            picBrilho.Image = new Bitmap(caminhoProcessadaBrilho);
            picCinza.Image = new Bitmap(caminhoProcessadaCinza);
            picRotacao.Image = new Bitmap(caminhoProcessadaCinza);

            contadorRotacao = 0;
        }

        // Botao simples que alterna entre a imagem cinza e a rotacionada.
        public void btnRotacionar_Click(object sender, EventArgs e)
        {
            contadorRotacao++;
            bool mostrarRotacionada = contadorRotacao % 2 == 1;

            if (mostrarRotacionada)
                picRotacao.Image = new Bitmap(caminhoProcessadaCinzaRotacionada);
            else
                picRotacao.Image = new Bitmap(caminhoProcessadaCinza);
        }

        // EVIDENCIA (RA7 p.16): cor = Color.FromArgb(r, g, b);
        public static Color CriarCor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        // I'(x,y) = I(x,y) + deltaB em cada canal, com clamp min(max(v,0),255).
        // EVIDENCIA (RA7 p.27 e p.29).
        public static Bitmap Iluminar(Bitmap img, int brilhoR, int brilhoG, int brilhoB)
        {
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
            return imagemNova;
        }

        // GrayLevel: g = (r*0.30) + (g*0.59) + (b*0.11) (RA7 p.21).
        public static Bitmap ImgGrayLevel(Bitmap img)
        {
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
            return imagemNova;
        }

        // Thresholding: se I(i,j) <= T entao 0, senao 255 (RA7 p.23).
        public static Bitmap Threshold(Bitmap img, int t)
        {
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
            return imagemNova;
        }

        // Cola imagemRecorte sobre imagemFundo na posicao (xPosicao, yPosicao),
        // ignorando pixels cuja cor esteja dentro do limiar da cor de fundo.
        // Mesmo if/else de comparacao de cor do slide RA7 p.18.
        public static void ColarImagem(Bitmap imagemFundo, Bitmap imagemRecorte, int xPosicao, int yPosicao, int limiar)
        {
            Color corFundo = imagemRecorte.GetPixel(0, 0);
            int largura = imagemRecorte.Width;
            int altura = imagemRecorte.Height;

            for (int i = 0; i <= largura - 1; i++)
            {
                for (int j = 0; j <= altura - 1; j++)
                {
                    int x = xPosicao + i;
                    int y = yPosicao + j;

                    // Verifica se esta dentro dos limites da imagem de fundo
                    if (x >= 0 && x <= imagemFundo.Width - 1 && y >= 0 && y <= imagemFundo.Height - 1)
                    {
                        int r = imagemRecorte.GetPixel(i, j).R;
                        int g = imagemRecorte.GetPixel(i, j).G;
                        int b = imagemRecorte.GetPixel(i, j).B;

                        // Se o pixel NAO for fundo, cola ele no fundo
                        if ((corFundo.R - limiar < r && corFundo.R + limiar > r &&
                             corFundo.G - limiar < g && corFundo.G + limiar > g &&
                             corFundo.B - limiar < b && corFundo.B + limiar > b) == false)
                        {
                            imagemFundo.SetPixel(x, y, CriarCor(r, g, b));
                        }
                    }
                }
            }
        }

        // Rotacao por transposicao de matriz: I'(x,y) = I(y,x) (RA6 p.20).
        public static Bitmap TransporImagem(Bitmap img)
        {
            Bitmap imagemNova = new Bitmap(img.Height, img.Width);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    imagemNova.SetPixel(j, i, img.GetPixel(i, j));
                }
            }
            return imagemNova;
        }
    }
}
