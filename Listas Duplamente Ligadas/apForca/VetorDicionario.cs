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
        /*
        NoDuplo<Dado> primeiro, ultimo,
                atual, anterior;   // é usado para percorrer a lista e mostrar
                                   // o nó que está sendo visitado a cada momento
        int quantosNos;
        bool primeiroAcessoDoPercurso;
        int numeroDoNoAtual;
        public enum Situacao
        {
            navegando, incluindo, pesquisando, editando, excluindo
        }

        Dicionario[] dados;   // vetor de Dicionário
        int qtosDados;        // tamanho lógico
        int posicaoAtual;     // índice que estamos visitando, no momento, no vetor dados
        Situacao situacaoAtual;

        public Situacao SituacaoAtual // esta propriedade permite acessar o atributo
        {                             // situacaoAtual para consulta e ajuste
            get => situacaoAtual;
            set => situacaoAtual = value;
        }
        public bool EstaVazio // permite à aplicação saber se o vetor dados está vazio
        {
            get => qtosDados <= 0; // se qtosDados <= 0, retorna true
        }
        public int PosicaoAtual // permite à aplicação saber qual a posição do registro
        {                       // visível na tela ou reposicionar o registro atualmente
            get => posicaoAtual;  // acessado
            set
            {
                if (value >= 0 && value < qtosDados)
                    posicaoAtual = value;
            }
        }
        public int Tamanho { get => qtosDados; }
        public VetorDicionario(int tamanhoMaximo)
        {
            dados = new Dicionario[tamanhoMaximo];
            qtosDados = 0;   // vetor vazio, no momento de sua instanciação
            posicaoAtual = -1;  // indica que não estamos posicionados em nenhum registro do vetor
            situacaoAtual = Situacao.navegando;
        }
        public Dicionario this[int indice]
        {
            get
            {
                if (indice < 0 || indice > qtosDados - 1)      // testamos a validade do indice passado
                    throw new Exception("Índice fora dos limites de armazenamento!");

                return dados[indice]; // se o índice passado é válido, retornamos o dado armazenado na posicao indice do vetor dados
            }
            set
            {
                if (indice >= 0 && indice < qtosDados)
                    dados[indice] = value;
                else
                    throw new Exception("Índice fora dos limites do vetor!");
            }
        }
        public void PosicionarNoPrimeiro()
        {
            
            if (!EstaVazio)
                posicaoAtual = 0; // primeira posição do vetor em uso
            else
                posicaoAtual = -1; // indica antes do vetor vazio
            

           
        }
        public void RetrocederPosicao()
        {
            
            if (posicaoAtual > 0)
                posicaoAtual--;
            

            
        }
        public void AvancarPosicao()
        {
            
            if (posicaoAtual < qtosDados - 1)
                posicaoAtual++;
           

            
        }
        public void PosicionarNoUltimo()
        {
            
            if (!EstaVazio)
                posicaoAtual = qtosDados - 1; // índice da última posição usada do vetor
            else
                posicaoAtual = -1; // indica antes do vetor vazio
            

            
        }
        public bool EstaNoInicio
        {
            get => posicaoAtual <= 0; // primeiro índice
        }
        public bool EstaNoFim
        {
            get => posicaoAtual >= qtosDados - 1; // último índice
        }
        public Dicionario RegistroAtual
        {
            get
            {
                if (posicaoAtual >= 0 && posicaoAtual < qtosDados)
                    return dados[posicaoAtual];
                return null; // Retorna null se não houver um registro válido na posição atual
            }
        }
        public Dicionario ValorDe(int indice)
        {
            if (indice >= 0 && indice < qtosDados)
                return dados[indice];

            throw new Exception("Índice fora dos limites de armazenamento!");
        }
        public void GravarDados(string nomeArquivo)
        {
            var arquivo = new StreamWriter(nomeArquivo);

            for (int indice = 0; indice < qtosDados; indice++)
                arquivo.WriteLine(dados[indice].FormatoDeArquivo());

            arquivo.Close();
        }
        public bool Existe(string palavraProcurada, out int onde)  // onde --> posicao onde achou ou onde deveria estar (inclusão)
        {
            onde = -1;   // o compilador exige que parâmetros out sejam iniciados

            bool achou = false;
            int inicio = 0;
            int fim = qtosDados - 1;
            while (!achou && inicio <= fim)  // não achou a chave e ainda temos onde procurar
            {
                onde = (inicio + fim) / 2;
                if (palavraProcurada == dados[onde].Palavra)
                    achou = true;  // a posição dessa palavra no vetor é o índice "onde"
                else
                  if (palavraProcurada.CompareTo(dados[onde].Palavra) < 0)
                    fim = onde - 1;
                else
                    inicio = onde + 1;
            }
            // tratar o caso em que nao encontramos a palavra procurada. Nessa situação,
            // a pesquisa acima (while) terminou com inicio > fim, e o índice inicio
            // indica a posição em que a palavra procurada deveria estar, caso existisse.
            // Fazemos o parâmetro onde receber inicio, para o caso de a aplicação
            // desejar incluir um registro com essa palavra, na posição que manteria o
            // vetor em ordem crescente das palavras

            if (!achou)       // ou seja, saimos do while porque inicio > fim
                onde = inicio;  // o parâmetro onde retorna a posição de uma eventual inclusão em ordem

            return achou; // por fim, retornamos se encontramos ou não a chave procurada.
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
            if (qtosDados > 0)
            {
                grade.RowCount = qtosDados; // ajustamos o numero de linhas do Grid
                for (int indice = 0; indice < qtosDados; indice++)
                {
                    grade.Rows[indice].HeaderCell.Value = indice + "";
                    grade[0, indice].Value = dados[indice].Palavra;
                    grade[1, indice].Value = dados[indice].Dica;
                }
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
        */


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
                // Não é mais necessário lidar com -1 como uma posição válida para "antes do vetor vazio"
                // Se o vetor estiver vazio, posicaoAtual deve ser -1 ou 0 dependendo da sua lógica.
                // Mas geralmente, se estaVazio, nao há posicaoAtual.
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
                if (indice < 0 || indice >= qtosDados) // Ajuste para >= qtosDados
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

        // Métodos de Navegação (ajustados para vetor)
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
            // Opcional: ordenar os dados após carregar se for sempre mantido em ordem
            // Se o arquivo já vem ordenado, não precisa. Se não, adicione:
            // Array.Sort(dados, 0, qtosDados); // Dicionario precisa implementar IComparable<Dicionario>
        }

        // Método para limpar o vetor
        public void Limpar()
        {
            for (int i = 0; i < qtosDados; i++)
            {
                dados[i] = null; // Libera as referências
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
