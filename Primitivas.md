# Primitivas do Projeto — 4º Bimestre (ICG)

Este documento explica cada primitiva implementada em `Form1.cs` no contexto do projeto: a composição de uma cena (paisagem + homem + balão + avião) seguida de pós-processamento da imagem final.

O projeto não usa placa gráfica nem álbum de imagens do OpenCV: a imagem é tratada como uma **matriz de pixels** manipulada diretamente com `GetPixel`/`SetPixel` sobre `System.Drawing.Bitmap`. Um pixel é representado pela estrutura `Color`, com canais R, G, B no intervalo `[0, 255]`.

Índice das primitivas:

| Primitiva | Função no projeto | Operação |
|---|---|---|
| `CriarCor` | apoio | cria um pixel `Color.FromArgb(r, g, b)` |
| `PixelEhFundo` | apoio | testa se um pixel pertence ao fundo do sprite |
| `ColarImagens` | composição da cena | cola os sprites sobre a paisagem |
| `ImgGrayLevel` | pós-processamento | converte a cena para níveis de cinza |
| `Threshold` | pós-processamento | binariza a imagem cinza (T = 126) |
| `Iluminar` | pós-processamento | soma brilho (+85) a cada canal |
| `TransporImagem` | pós-processamento | transpõe a matriz (rotação 90°) |

---

## 1. `CriarCor(int r, int g, int b)` — apoio

```csharp
public static Color CriarCor(int r, int g, int b)
{
    return Color.FromArgb(r, g, b);
}
```

Função utilitária que padroniza a criação de pixels no projeto: toda escrita de pixel (`SetPixel`) passa por ela. A representação de cor aqui é o modelo RGB aditivo, onde cada canal assume valores de 0 (ausência) a 255 (intensidade máxima). `(0,0,0)` é preto, `(255,255,255)` é branco e a diagonal R = G = B corresponde aos tons de cinza.

---

## 2. `PixelEhFundo(Color pixel, Color corFundo, int limiar)` — apoio

```csharp
public static bool PixelEhFundo(Color pixel, Color corFundo, int limiar)
{
    return corFundo.R - limiar < pixel.R && corFundo.R + limiar > pixel.R &&
           corFundo.G - limiar < pixel.G && corFundo.G + limiar > pixel.G &&
           corFundo.B - limiar < pixel.B && corFundo.B + limiar > pixel.B;
}
```

É o teste de transparência do projeto, dobrando como máscara de recorte (chroma key). Cada sprite do projeto é um JPG sem canal alfa, então o "transparente" precisa ser deduzido: o programa **amostra a cor de fundo do sprite no pixel (0,0)** e considera fundo qualquer pixel cujos três canais estejam, cada um, dentro de ±`limiar` dessa cor amostrada.

Geometricamente, o teste define um **cubo de tolerância** no espaço RGB centrado em `corFundo`, com aresta `2 * limiar`. O limiar é calibrado por sprite (`limiarHomem = 12`, `limiarBalao = 32`, `limiarAviao = 45`) — sprites com fundo menos uniforme pedem limiar maior.

Limitação conhecida: se um objeto dentro do sprite tiver cor parecida com o fundo dele, esse pixel também será descartado (furo no objeto); e se o fundo variar além do limiar, o resíduo sobrevive e é colado junto.

---

## 3. `ColarImagens(...)` — composição da cena

```csharp
public static void ColarImagens(string caminhoFundo,
                                string caminhoHomem, int limiarHomem,
                                string caminhoBalao, int limiarBalao,
                                string caminhoAviao, int limiarAviao,
                                string caminhoSaida)
```

É a primitiva central do projeto: recebe apenas **caminhos de arquivo** (não bitmaps) e produz a imagem final da cena. Passos:

1. Carrega a paisagem (`Im.jpg`) em um `Bitmap` — ela funciona como o **framebuffer** de destino.
2. Para cada sprite, em sequência (homem → balão → avião):
   - Carrega o sprite e amostra `corFundo = GetPixel(0, 0)`.
   - Percorre todos os pixels `(i, j)` do sprite com um loop duplo.
   - **Translada** o pixel para sua posição na cena: homem em `(300, 500)`, balão em `(500, 0)` e avião em `(200, 0)`.
   - **Clipping**: escreve somente se `(x, y)` está dentro dos limites do fundo — evita pintar fora da paisagem.
   - **Transparência**: descarta o pixel se `PixelEhFundo` for verdadeiro.
   - Caso contrário, **sobrescreve** o pixel do fundo: `fundo.SetPixel(x, y, pixel)`.
3. Salva o resultado em `Processada.jpg`.

Características importantes:

- **Não há blending**: o pixel do sprite ou substitui integralmente o do fundo ou é descartado (operação de overwrite, não de mistura).
- **A ordem de colagem define as camadas**: quem é colado depois cobre quem veio antes onde houver sobreposição (avião sobre balão, por exemplo).
- Essa técnica equivale a uma "draw image" por software, com máscara binária por limiar em vez de canal alfa.

---

## 4. `ImgGrayLevel(string entrada, string saida)` — conversão para cinza

```csharp
int cinza = (int)(r * 0.3 + g * 0.59 + b * 0.11);
imagemNova.SetPixel(i, j, CriarCor(cinza, cinza, cinza));
```

Converte a cena colorida em uma imagem de **níveis de cinza**: cada pixel de saída tem R = G = B (o que o coloca sobre a diagonal cinza do cubo RGB). O valor do cinza não é a média simples dos canais, e sim uma **média ponderada de luminância** — `0.30 R + 0.59 G + 0.11 B` — que imita a sensibilidade distinta do olho humano (muito maior para o verde, menor para o azul). Com média simples, cores vivas como verde-limão e azul-claro apareceria com brilho errado na escala de cinza.

Como a soma dos pesos é exatamente 1, o resultado nunca excede 255 e **não precisa de clamp**. É o passo que prepara a entrada do `Threshold`, pois a binarização exige um único valor de intensidade por pixel.

---

## 5. `Threshold(string entrada, string saida, int t)` — binarização

```csharp
if (cinza <= t)
    imagemNova.SetPixel(i, j, CriarCor(0, 0, 0));      // preto
else
    imagemNova.SetPixel(i, j, CriarCor(255, 255, 255)); // branco
```

Reduz a imagem a **dois níveis** (0 ou 255) comparando o nível de cinza de cada pixel com o limiar `t`. No projeto é chamado com **T = 126** sobre a já-convertida `ProcessadaCinza.jpg`, gerando a silhueta preto-e-branco da cena.

É uma operação de **quantização extrema** (de 256 níveis para 2): útil para segmentar o que é claro/escuro, mas destrói todo o detalhe intermediário. O resultado não é exibido por padrão na tela, apenas salvo em `ProcessadaBinaria.jpg`. Nota: internamente ela recalcula o cinza com os mesmos pesos da `ImgGrayLevel` — ao aplicá-la sobre a imagem já cinza, os canais são iguais e o cálculo devolve o próprio valor.

---

## 6. `Iluminar(string entrada, string saida, int brilhoR, int brilhoG, int brilhoB)` — brilho

```csharp
imagemNova.SetPixel(i, j, CriarCor(
    Math.Max(0, Math.Min(255, r + brilhoR)),
    Math.Max(0, Math.Min(255, g + brilhoG)),
    Math.Max(0, Math.Min(255, b + brilhoB))));
```

Soma um delta de brilho a **cada canal** do pixel (operação ponto a ponto sobre a imagem): `I'(i,j) = I(i,j) + Δ`, com Δ diferente por canal, o que permite deslocar a cor no espaço RGB. No projeto é chamado com `(85, 85, 85)` sobre a cena colorida — um deslocamento uniforme que **clareia** a imagem inteira.

Como `r + 85` pode ultrapassar 255 e valores negativos não existem em `Color`, é preciso o **clamp** `min(max(v, 0), 255)`: o resultado satura em branco (255) nos pixels claros e em preto (0) nos escuros. Isso é o mesmo efeito de "queimar" os realces numa foto estourada de brilho. Vale notar que a imagem de entrada aqui é a cena composta original (`Processada.jpg`), não a cinza.

---

## 7. `TransporImagem(string entrada, string saida)` — rotação por transposição

```csharp
Bitmap imagemNova = new Bitmap(img.Height, img.Width);
...
imagemNova.SetPixel(j, i, img.GetPixel(i, j));
```

Implementa a transformação geométrica `I'(x, y) = I(y, x)` — a **transposição da matriz** de pixels. Trocando linhas por colunas, a imagem fica rotacionada 90° (e espelhada), e as dimensões trocam: a imagem de saída é criada com `img.Height × img.Width`.

No projeto é aplicada sobre `ProcessadaCinza.jpg` e o resultado é alternado no picture box `picRotacao` pelo botão `btnRotacionar_Click`, que alterna a exibição entre a versão cinza e a rotacionada usando o contador `contadorRotacao` (par = cinza, ímpar = rotacionada).

Essa é a forma mais barata de rotação: não envolve trigonometria nem interpolação — só inverte os índices na escrita. (Uma rotação por ângulo arbitrário θ pediria a matriz de rotação com cos/sin e reamostragem dos pixels.)

---

## 8. Fluxo completo (`btnProcessar_Click`)

Um único botão executa o pipeline todo e exibe o resultado:

```
paisagem + homem(300,500) + balão(500,0) + avião(200,0)
        │ (ColarImagens, máscara por limiar)
        ▼
   Processada.jpg ──┬─► ImgGrayLevel ──► Threshold(126) ──► ProcessadaBinaria.jpg
                    ├─► Iluminar(+85)                     ──► ProcessadaBrilho.jpg
                    ├─► ImgGrayLevel                      ──► ProcessadaCinza.jpg
                    └─► (cinza) TransporImagem            ──► ProcessadaCinzaRotacionada.jpg
```

As imagens são carregadas nos picture boxes com `picX.Load(caminho)` — o método exigido pelo enunciado para exibição.

## Visão geral do repositório

O projeto é um WinForms em C# (.NET 10) — a lógica toda está em **`Form1.cs`**; `Program.cs` apenas inicia o `Form1`. Não há OpenGL/shader/framebuffer de hardware: usa `System.Drawing.Bitmap` como "framebuffer" (canvas em software), e todas as primitivas são feitas pixel a pixel via `GetPixel`/`SetPixel`.
