using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace apListaLigada
{
  public partial class FrmAlunos : Form
  {
        //ListaDupla<Aluno> lista1;

        private ListaDupla<PalavraDica> listaPalavras;
        private int posicaoAtual = 0;
        private int totalPalavras = 0;
        
    public FrmAlunos()
    {
        InitializeComponent();
        //lista1 = new ListaDupla<Aluno>();

        listaPalavras = new ListaDupla<PalavraDica>();
        FazerLeitura(ref listaPalavras);
    }


    // Nao sei o que fazer nesse evento de botao
    private void btnLerArquivo1_Click(object sender, EventArgs e)
    {
            //FazerLeitura(ref listaPalavras);       // FazerLeitura(ref lista1);
    }

    private void FazerLeitura(ref ListaDupla<PalavraDica> qualLista)             // ref ListaDupla<Aluno> qualLista
        {
            // instanciar a lista de palavras e dicas
            // pedir ao usuário o nome do arquivo de entrada
            // abrir esse arquivo e lê-lo linha a linha
            // para cada linha, criar um objeto da classe de Palavra e Dica
            // e inseri-0lo no final da lista duplamente ligada



            /*
            //listaPalavras = new ListaDupla<PalavraDica>(); // Recria a lista


            qualLista = new ListaDupla<PalavraDica>();

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
                openFileDialog.Title = "Selecione o arquivo de palavras e dicas";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        //string[] linhas = File.ReadAllLines(openFileDialog.FileName);

                        string[] linhas = File.ReadAllLines(openFileDialog.FileName)
                                            .Where(l => !string.IsNullOrWhiteSpace(l))
                                            .ToArray();

                        // Ignorar a primeira linha (cabeçalho)
                        for (int i = 1; i < linhas.Length; i++)
                        {
                            string linha = linhas[i];

                            if (linha.Length >= 30)
                            {
                                string palavra = linha.Substring(0, 30).Trim();
                                string dica = linha.Substring(30).Trim();

                                PalavraDica nova = new PalavraDica(palavra, dica);
                                qualLista.InserirAposFim(nova);
                            }
                            else
                            {
                                MessageBox.Show($"Linha {i + 1} inválida. Linha muito curta: \"{linha}\"", "Aviso",
                                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                        }

                        totalPalavras = qualLista.QuantosNos;
                        if (totalPalavras > 0)
                        {
                            qualLista.PosicionarNoInicio();
                            ExibirRegistroAtual();
                        }
                        else
                        {
                            MessageBox.Show("O arquivo selecionado não contém palavras válidas.", "Aviso",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao processar arquivo: {ex.Message}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo foi selecionado. Por favor, selecione um arquivo para continuar.",
                                  "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //MessageBox.Show($"Total de palavras lidas: {qualLista.QuantosNos}");
            */


            qualLista = new ListaDupla<PalavraDica>();  // Aqui pode recriar SIM porque é ref!

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
                openFileDialog.Title = "Selecione o arquivo de palavras e dicas";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string[] linhas = File.ReadAllLines(openFileDialog.FileName)
                                              .Where(l => !string.IsNullOrWhiteSpace(l))
                                              .ToArray();

                        for (int i = 1; i < linhas.Length; i++)
                        {
                            string linha = linhas[i];

                            if (linha.Length >= 30)
                            {
                                string palavra = linha.Substring(0, 30).Trim();
                                string dica = linha.Substring(30).Trim();

                                //PalavraDica nova = new PalavraDica(palavra, dica);
                                //qualLista.InserirAposFim(nova);

                                // Verifica se a palavra não está vazia
                                if (!string.IsNullOrWhiteSpace(palavra))
                                {
                                    PalavraDica nova = new PalavraDica(palavra, dica);
                                    qualLista.InserirAposFim(nova);
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Linha {i + 1} inválida. Linha muito curta: \"{linha}\"", "Aviso",
                                                 MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }

                        totalPalavras = qualLista.QuantosNos;
                        if (totalPalavras > 0)
                        {
                            qualLista.PosicionarNoInicio();
                            ExibirRegistroAtual();
                        }
                        else
                        {
                            MessageBox.Show("O arquivo selecionado não contém palavras válidas.", "Aviso",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Erro ao processar arquivo: {ex.Message}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Nenhum arquivo foi selecionado. Por favor, selecione um arquivo para continuar.",
                                  "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            //MessageBox.Show($"Total de linhas lidas: {qualLista.QuantosNos}");        // $"Total de palavras lidas: {qualLista.QuantosNos}"

        }

    private void btnIncluir_Click(object sender, EventArgs e)
    {
      // se o usuário digitou palavra e dica:
      // criar objeto da classe Palavra e Dica para busca
      // tentar incluir em ordem esse objeto na lista1
      // se não incluiu (já existe) avisar o usuário
    }


    private void btnBuscar_Click(object sender, EventArgs e)
    {
      // se a palavra digitada não é vazia:
      // criar um objeto da classe de Palavra e Dica para busca
      // se a palavra existe na lista1, posicionar o ponteiro atual nesse nó e exibir o registro atual
      // senão, avisar usuário que a palavra não existe
      // exibir o nó atual
    }

    private void btnExcluir_Click(object sender, EventArgs e)
    {
      // para o nó atualmente visitado e exibido na tela:
      // perguntar ao usuário se realmente deseja excluir essa palavra e dica
      // se sim, remover o nó atual da lista duplamente ligada e exibir o próximo nó
      // se não, manter como está
    }

    private void FrmAlunos_FormClosing(object sender, FormClosingEventArgs e)
    {
      // solicitar ao usuário que escolha o arquivo de saída
      // percorrer a lista ligada e gravar seus dados no arquivo de saída
    }

    private void ExibirDados(ListaDupla<PalavraDica> aLista, ListBox lsb, Direcao qualDirecao)         //   ListaDupla<Aluno> aLista, ListBox lsb, Direcao qualDirecao
    {
      //lsb.Items.Clear();
      //var dadosDaLista = aLista.Listagem(qualDirecao);
      //foreach (Aluno aluno in dadosDaLista)
        //lsb.Items.Add(aluno);



    }

    private void tabControl1_Enter(object sender, EventArgs e)
    {
      rbFrente.PerformClick();
    }

    private void rbFrente_Click(object sender, EventArgs e)
    {
      //ExibirDados(lista1, lsbDados, Direcao.paraFrente);
    }

    private void rbTras_Click(object sender, EventArgs e)
    {
      //ExibirDados(lista1, lsbDados, Direcao.paraTras);
    }

    private void FrmAlunos_Load(object sender, EventArgs e)
    {
      // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na lista1
      // posicionar o ponteiro atual no início da lista duplamente ligada
      // Exibir o Registro Atual;
    }

    private void btnInicio_Click(object sender, EventArgs e)
    {
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            listaPalavras.PosicionarNoInicio();
            posicaoAtual = 0;
            ExibirRegistroAtual();

            
        }

    private void btnAnterior_Click(object sender, EventArgs e)
    {
            // Retroceder o ponteiro atual para o nó imediatamente anterior 
            // Exibir o Registro Atual;


            listaPalavras.Retroceder();
            if (posicaoAtual > 0) posicaoAtual--;
            ExibirRegistroAtual();

            

        }

    private void btnProximo_Click(object sender, EventArgs e)
    {
            // Retroceder o ponteiro atual para o nó seguinte 
            // Exibir o Registro Atual;

            listaPalavras.Avancar();
            if (posicaoAtual < totalPalavras - 1) posicaoAtual++;
            ExibirRegistroAtual();
        }

    private void btnFim_Click(object sender, EventArgs e)
    {
            // posicionar o ponteiro atual no último nó da lista 
            // Exibir o Registro Atual;

            listaPalavras.PosicionarNoFinal();
            posicaoAtual = totalPalavras - 1;
            ExibirRegistroAtual();
        }

    private void ExibirRegistroAtual()
    {
            // se a lista não está vazia:
            // acessar o nó atual e exibir seus campos em txtDica e txtPalavra
            // atualizar no status bar o número do registro atual / quantos nós na lista

            if (listaPalavras.Atual != null)
            {
                //txtPalavra.Text = listaPalavras.Atual.Info.Palavra;
                //txtDica.Text = listaPalavras.Atual.Info.Dica;
                //lblPosicao.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";

                txtRA.Text = listaPalavras.Atual.Info.Palavra;
                txtNome.Text = listaPalavras.Atual.Info.Dica;
                slRegistro.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";
            }

        }

    private void btnEditar_Click(object sender, EventArgs e)
    {
      // alterar a dica e guardar seu novo valor no nó exibido
    }

    private void btnSair_Click(object sender, EventArgs e)
    {
      Close();
    }

    private void btnCancelar_Click(object sender, EventArgs e)
    {

    }
  }
}
