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
            Bitmap paisagem = new Bitmap(@"c:\Imagens\Im.jpg");

            // Enunciado item (a): remove o fundo de B, C e D usando a cor do pixel (0,0).
            Bitmap aviao = RemoverFundo(new Bitmap(@"c:\Imagens\Aviao2.jpg"));
            Bitmap balao = RemoverFundo(new Bitmap(@"c:\Imagens\Balao.jpg"));
            Bitmap homem = RemoverFundo(new Bitmap(@"c:\Imagens\homem.jpg"));

            // Cola cada objeto na cena final, nas posicoes do codigo do colega.
            aviao = ColarImagem(aviao, paisagem.Width, paisagem.Height, 200, 0);
            balao = ColarImagem(balao, paisagem.Width, paisagem.Height, 500, 0);
            homem = ColarImagem(homem, paisagem.Width, paisagem.Height, 300, 500);

            // Enunciado itens 1 e (k): A + B + C + D = E numa unica funcao.
            Bitmap cenaFinal = UnirImagens(paisagem, aviao, balao, homem);

            // Enunciado itens (m), (e) e (o): quatro saidas .jpg em c:\Imagens.
            cenaFinal.Save(@"c:\Imagens\E_original.jpg");
            Bitmap imagemBrilho = Iluminar(cenaFinal, 85);     // item (h): brilho 85
            imagemBrilho.Save(@"c:\Imagens\E_brilho85.jpg");
            Bitmap imagemCinza = ImgGrayLevel(cenaFinal);       // item (i)
            imagemCinza.Save(@"c:\Imagens\E_cinza.jpg");
            Bitmap imagemBinaria = Threshold(imagemCinza, 126); // item (j): T = 126
            imagemBinaria.Save(@"c:\Imagens\E_binaria.jpg");
            Rotacionar(imagemCinza).Save(@"c:\Imagens\E_cinza_rotacionada.jpg"); // item (n)

            // Mostra as 4 imagens principais na tela (item g: opcional, mas agora ativo).
            picOriginal.Image = cenaFinal;
            picBrilho.Image = imagemBrilho;
            picCinza.Image = imagemCinza;
            picRotacao.Image = imagemBinaria;
        }

        // Botao simples que aplica a primitiva de rotacao a imagem atual do picRotacao.
        // Cada clique rotaciona mais 90° (transposicao de matriz do slide RA6 p.20).
        public void btnRotacionar_Click(object sender, EventArgs e)
        {
            if (picRotacao.Image == null) return;
            Bitmap imagemAtual = new Bitmap(picRotacao.Image);
            Bitmap imagemRotacionada = Rotacionar(imagemAtual);
            imagemRotacionada.Save(@"c:\Imagens\E_rotacionada.jpg");
            picRotacao.Image = imagemRotacionada;
        }

        // EVIDENCIA (RA7 p.16): cor = Color.FromArgb(r, g, b);
        public static Color CriarCor(int r, int g, int b)
        {
            return Color.FromArgb(r, g, b);
        }

        // I'(x,y) = I(x,y) + deltaB com clamp min(max(v,0),255) (RA7 p.27 e p.29).
        public static Bitmap Iluminar(Bitmap img, int deltaB)
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
                        Math.Max(0, Math.Min(255, r + deltaB)),
                        Math.Max(0, Math.Min(255, g + deltaB)),
                        Math.Max(0, Math.Min(255, b + deltaB))));
                }
            }
            return imagemNova;
        }

        // g = (r*0.30) + (g*0.59) + (b*0.11) (RA7 p.21).
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

        // Se I(i,j) <= T entao 0, senao 255 (RA7 p.23).
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

        // Remove o fundo: pega a cor do pixel (0,0) e apaga os pixels com a mesma
        // cor (com tolerancia 30), mantendo o objeto. Mesmo if/else do slide RA7 p.18.
        public static Bitmap RemoverFundo(Bitmap img)
        {
            Color corFundo = img.GetPixel(0, 0);
            Bitmap imagemNova = new Bitmap(img.Width, img.Height);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int r = img.GetPixel(i, j).R;
                    int g = img.GetPixel(i, j).G;
                    int b = img.GetPixel(i, j).B;
                    if (Math.Abs(r - corFundo.R) < 30 && Math.Abs(g - corFundo.G) < 30 && Math.Abs(b - corFundo.B) < 30)
                        imagemNova.SetPixel(i, j, CriarCor(0, 0, 0));
                    else
                        imagemNova.SetPixel(i, j, CriarCor(r, g, b));
                }
            }
            return imagemNova;
        }

        // Uniao A + B + C + D = E numa unica funcao (item k): fundo preto (0,0,0)
        // nao contribui, objeto (cor != preta) entra na soma (RA7 p.15 e p.16).
        public static Bitmap UnirImagens(Bitmap paisagem, Bitmap aviao, Bitmap balao, Bitmap homem)
        {
            Bitmap imagemNova = new Bitmap(paisagem.Width, paisagem.Height);
            for (int i = 0; i <= paisagem.Width - 1; i++)
            {
                for (int j = 0; j <= paisagem.Height - 1; j++)
                {
                    Color corFinal = paisagem.GetPixel(i, j);
                    Color pixelAviao = aviao.GetPixel(i, j);
                    if (pixelAviao.R != 0 || pixelAviao.G != 0 || pixelAviao.B != 0)
                        corFinal = pixelAviao;
                    Color pixelBalao = balao.GetPixel(i, j);
                    if (pixelBalao.R != 0 || pixelBalao.G != 0 || pixelBalao.B != 0)
                        corFinal = pixelBalao;
                    Color pixelHomem = homem.GetPixel(i, j);
                    if (pixelHomem.R != 0 || pixelHomem.G != 0 || pixelHomem.B != 0)
                        corFinal = pixelHomem;
                    imagemNova.SetPixel(i, j, corFinal);
                }
            }
            return imagemNova;
        }

        // Rotacao por transposicao de matriz: I'(x,y) = I(y,x) (RA6 p.20).
        public static Bitmap Rotacionar(Bitmap img)
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

        // Cola a imagem dentro de um Bitmap do tamanho da cena, na posicao
        // x' = x + tx, y' = y + ty (RA6 p.12). Fundo preto (0,0,0) nao contribui.
        public static Bitmap ColarImagem(Bitmap img, int larguraCena, int alturaCena, int tx, int ty)
        {
            Bitmap imagemNova = new Bitmap(larguraCena, alturaCena);
            for (int i = 0; i <= img.Width - 1; i++)
            {
                for (int j = 0; j <= img.Height - 1; j++)
                {
                    int x = i + tx;
                    int y = j + ty;
                    if (x >= 0 && x <= larguraCena - 1 && y >= 0 && y <= alturaCena - 1)
                        imagemNova.SetPixel(x, y, img.GetPixel(i, j));
                }
            }
            return imagemNova;
        }
    }
}
