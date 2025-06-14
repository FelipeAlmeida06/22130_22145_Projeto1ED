// Nome: Felipe Antônio de Oliveira Almeida     RA: 22130
// Nome: Miguel de Castro Chaga Silva           RA: 22145

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace apListaLigada
{
    public class VetorDicionario
    {
        Dicionario[] dados;
        int qtosDados;
        int posicaoAtual;
        Situacao situacaoAtual;

        // Propriedades e enum (mantidas como estão, ou com pequenos ajustes)
        public enum Situacao
        {
            navegando, incluindo, pesquisando, editando, excluindo
        }

        public Situacao SituacaoAtual
        {
            get => situacaoAtual;
            set => situacaoAtual = value;
        }

        public bool EstaVazio
        {
            get => qtosDados <= 0;
        }

        public int PosicaoAtual
        {
            get => posicaoAtual;
            set
            {
                if (value >= 0 && value < qtosDados)
                    posicaoAtual = value;
            }
        }

        public int Tamanho { get => qtosDados; }

        public Dicionario RegistroAtual
        {
            get
            {
                if (posicaoAtual >= 0 && posicaoAtual < qtosDados)
                    return dados[posicaoAtual];
                return null;
            }
        }

        // Construtor
        public VetorDicionario(int tamanhoMaximo)
        {
            dados = new Dicionario[tamanhoMaximo];
            qtosDados = 0;
            posicaoAtual = -1;
            situacaoAtual = Situacao.navegando;
        }

        // Indexador
        public Dicionario this[int indice]
        {
            get
            {
                if (indice < 0 || indice >= qtosDados)
                    throw new Exception("Índice fora dos limites de armazenamento!");
                return dados[indice];
            }
            set
            {
                if (indice >= 0 && indice < qtosDados)
                    dados[indice] = value;
                else
                    throw new Exception("Índice fora dos limites do vetor!");
            }
        }

        // Métodos de Navegação
        public void PosicionarNoPrimeiro()
        {
            if (!EstaVazio)
                posicaoAtual = 0;
            else
                posicaoAtual = -1;
        }

        public void RetrocederPosicao()
        {
            if (!EstaVazio && posicaoAtual > 0)
                posicaoAtual--;
        }

        public void AvancarPosicao()
        {
            if (!EstaVazio && posicaoAtual < qtosDados - 1)
                posicaoAtual++;
        }

        public void PosicionarNoUltimo()
        {
            if (!EstaVazio)
                posicaoAtual = qtosDados - 1;
            else
                posicaoAtual = -1;
        }

        public bool EstaNoInicio
        {
            get => EstaVazio || posicaoAtual <= 0;
        }

        public bool EstaNoFim
        {
            get => posicaoAtual >= qtosDados - 1 || EstaVazio; // Se está vazio, também está no "fim" (não tem próximo)
        }


        // Método para carregar dados de um arquivo
        public void LerDadosDeArquivo(string nomeArquivo)
        {
            qtosDados = 0; // Zera o contador de dados antes de carregar
            posicaoAtual = -1; // Reseta a posição atual

            using (StreamReader arq = new StreamReader(nomeArquivo))
            {
                while (!arq.EndOfStream && qtosDados < dados.Length)
                {
                    Dicionario novaEntrada = new Dicionario();
                    novaEntrada.LerDados(arq); // Assume que Dicionario tem LerDados(StreamReader)
                    Incluir(novaEntrada); // Usa o método Incluir existente para adicionar ao final
                }
            }
        }

        public void Limpar()
        {
            for (int i = 0; i < qtosDados; i++)
            {
                dados[i] = null;
            }
            qtosDados = 0;
            posicaoAtual = -1;
            situacaoAtual = Situacao.navegando;
        }
        

        public void Excluir(int posicao)
        {
            if (posicao < 0 || posicao >= qtosDados)                 // conferimos se a posição passada para exclusão
                throw new Exception("Posição de exclusão inválida!");  // está dentro do intervalo 0 a qtosDados-1

            qtosDados--;
            for (int indice = posicao; indice < qtosDados; indice++)
                dados[indice] = dados[indice + 1];

            dados[qtosDados] = null;
        }
        public void Incluir(Dicionario novoValor)  // inclui ao final do vetor
        {
            if (qtosDados < dados.Length)
            {
                dados[qtosDados] = novoValor;
                qtosDados++;
            }
            else
                throw new Exception("Espaço insuficiente para armazenamento dos dados!");
        }
        public void Incluir(Dicionario novoValor, int posicaoDeInclusao)  // incluir novoValor no índice posicaoDeInclusao 
        {
            if (posicaoDeInclusao < 0 && posicaoDeInclusao > qtosDados)
                throw new Exception("Posiçao de inclusao é inválida!");

            if (qtosDados >= dados.Length)
                throw new Exception("Espaço de armazenamento insuficente!");

            for (int indice = qtosDados; indice > posicaoDeInclusao; indice--)
                dados[indice] = dados[indice - 1];

            dados[posicaoDeInclusao] = novoValor;
            qtosDados++;  // expande tamanho lógico do vetor
        }
        public void Alterar(int indice, Dicionario novoDado)
        {
            if (indice >= 0 && indice < qtosDados)
                dados[indice] = novoDado;

            throw new Exception("Índice fora dos limites do vetor!");
        }
        public void ExibirDados(ListBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtosDados; indice++)
                lista.Items.Add(dados[indice].ToString());
            Application.DoEvents();
        }
        public void ExibirDados(ComboBox lista)
        {
            lista.Items.Clear();
            for (int indice = 0; indice < qtosDados; indice++)
                lista.Items.Add(dados[indice].ToString());
            Application.DoEvents();
        }
        public void ExibirDados()
        {
            Console.Clear();
            for (int indice = 0; indice < qtosDados; indice++)
                Console.WriteLine($"{indice} - {dados[indice]}");
        }
        public void ExibirDados(DataGridView grade)
        {
            grade.Rows.Clear(); // Limpa as linhas existentes no DataGridView antes de preencher

            if (qtosDados > 0)
            {
                grade.RowCount = qtosDados; // ajustamos o numero de linhas do Grid
                for (int indice = 0; indice < qtosDados; indice++)
                {
                    grade.Rows[indice].HeaderCell.Value = (indice + 1).ToString(); // Número da linha começando de 1
                    grade[0, indice].Value = dados[indice].Palavra; // Coluna 0 para Palavra
                    grade[1, indice].Value = dados[indice].Dica;    // Coluna 1 para Dica
                }
            }
            else
            {
                grade.RowCount = 1; // Garante que não há linhas se o vetor estiver vazio
            }
        }
        public void ExibirDados(TextBox lista)
        {
            lista.Text = "";
            lista.Multiline = true;
            lista.ScrollBars = ScrollBars.Both;
            for (int indice = 0; indice < qtosDados; indice++)
                lista.AppendText(dados[indice] + Environment.NewLine);
            Application.DoEvents();
        }
    }
}
