namespace Projeto_4_bim
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Button btnProcessar;
        private Button btnRotacionar;
        private PictureBox picOriginal;
        private PictureBox picBrilho;
        private PictureBox picCinza;
        private PictureBox picRotacao;

        /// <summary>
        ///  Clean up any resources.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnProcessar = new Button();
            btnRotacionar = new Button();
            picOriginal = new PictureBox();
            picBrilho = new PictureBox();
            picCinza = new PictureBox();
            picRotacao = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)picOriginal).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picBrilho).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCinza).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picRotacao).BeginInit();
            SuspendLayout();
            // 
            // btnProcessar
            // 
            btnProcessar.Location = new Point(12, 12);
            btnProcessar.Name = "btnProcessar";
            btnProcessar.Size = new Size(220, 45);
            btnProcessar.TabIndex = 0;
            btnProcessar.Text = "Processar (A+B+C+D=E)";
            btnProcessar.UseVisualStyleBackColor = true;
            btnProcessar.Click += btnProcessar_Click;
            // 
            // btnRotacionar
            // 
            btnRotacionar.Location = new Point(250, 12);
            btnRotacionar.Name = "btnRotacionar";
            btnRotacionar.Size = new Size(140, 45);
            btnRotacionar.TabIndex = 5;
            btnRotacionar.Text = "Rotacionar";
            btnRotacionar.UseVisualStyleBackColor = true;
            btnRotacionar.Click += btnRotacionar_Click;
            // 
            // picOriginal
            // 
            picOriginal.BorderStyle = BorderStyle.FixedSingle;
            picOriginal.Location = new Point(12, 70);
            picOriginal.Name = "picOriginal";
            picOriginal.Size = new Size(300, 225);
            picOriginal.SizeMode = PictureBoxSizeMode.Zoom;
            picOriginal.TabIndex = 1;
            picOriginal.TabStop = false;
            // 
            // picBrilho
            // 
            picBrilho.BorderStyle = BorderStyle.FixedSingle;
            picBrilho.Location = new Point(330, 70);
            picBrilho.Name = "picBrilho";
            picBrilho.Size = new Size(300, 225);
            picBrilho.SizeMode = PictureBoxSizeMode.Zoom;
            picBrilho.TabIndex = 2;
            picBrilho.TabStop = false;
            // 
            // picCinza
            // 
            picCinza.BorderStyle = BorderStyle.FixedSingle;
            picCinza.Location = new Point(648, 70);
            picCinza.Name = "picCinza";
            picCinza.Size = new Size(300, 225);
            picCinza.SizeMode = PictureBoxSizeMode.Zoom;
            picCinza.TabIndex = 3;
            picCinza.TabStop = false;
            // 
            // picRotacao
            // 
            picRotacao.BorderStyle = BorderStyle.FixedSingle;
            picRotacao.Location = new Point(966, 70);
            picRotacao.Name = "picRotacao";
            picRotacao.Size = new Size(300, 225);
            picRotacao.SizeMode = PictureBoxSizeMode.Zoom;
            picRotacao.TabIndex = 4;
            picRotacao.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1284, 311);
            Controls.Add(picRotacao);
            Controls.Add(picCinza);
            Controls.Add(picBrilho);
            Controls.Add(picOriginal);
            Controls.Add(btnRotacionar);
            Controls.Add(btnProcessar);
            Name = "Form1";
            Text = "Projeto 4 BIM - Processamento de Imagens";
            ((System.ComponentModel.ISupportInitialize)picOriginal).EndInit();
            ((System.ComponentModel.ISupportInitialize)picBrilho).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCinza).EndInit();
            ((System.ComponentModel.ISupportInitialize)picRotacao).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
