using System;
using System.Collections.Generic;
using System.Drawing;
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
        }


        // Nao sei o que fazer nesse evento de botao
        //private void btnLerArquivo1_Click(object sender, EventArgs e)
        //{
        //        //FazerLeitura(ref listaPalavras);       // FazerLeitura(ref lista1);
        //}

    private void btnFecharArquivo_Click(object sender, EventArgs e)
    {
            try
            {
                // Confirmação antes de fechar (se houver dados)
                if (listaPalavras != null && listaPalavras.QuantosNos > 0)
                {
                    DialogResult resposta = MessageBox.Show(
                        "Tem certeza que deseja fechar o arquivo atual?",
                        "Aviso",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);

                    if (resposta != DialogResult.Yes)
                        return;
                }

                // Limpa a lista de palavras
                listaPalavras = new ListaDupla<PalavraDica>();

                // Reseta os contadores
                posicaoAtual = 0;
                totalPalavras = 0;

                // Limpa os campos de exibição
                txtRA.Text = "";
                txtNome.Text = "";

                // Limpa o ListBox de dados
                lsbDados.Items.Clear();

                // Atualiza o status
                slRegistro.Text = "Registro: 0/0";

                // Desabilita os botões de navegação
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;

                // Habilita o botão de abrir arquivo
                btnLerArquivo.Enabled = true;

                // Mostra mensagem de sucesso
                MessageBox.Show("Arquivo fechado com sucesso",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);     // .\nTodos os dados foram limpos."
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao fechar arquivo: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
            }
        }

    private void btnLerArquivo_Click(object sender, EventArgs e)
    {
         FazerLeitura(ref listaPalavras);
    }

        private void FazerLeitura(ref ListaDupla<PalavraDica> qualLista)             // ref ListaDupla<Aluno> qualLista
        {
            // instanciar a lista de palavras e dicas
            // pedir ao usuário o nome do arquivo de entrada
            // abrir esse arquivo e lê-lo linha a linha
            // para cada linha, criar um objeto da classe de Palavra e Dica
            // e inseri-0lo no final da lista duplamente ligada


            qualLista = new ListaDupla<PalavraDica>();

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

        
    private bool modoEdicao = false; // Variável para controlar o estado
        
    private void btnIncluir_Click(object sender, EventArgs e)
    {
            // se o usuário digitou palavra e dica:
            // criar objeto da classe Palavra e Dica para busca
            // tentar incluir em ordem esse objeto na lista1
            // se não incluiu (já existe) avisar o usuário



            // Habilita o modo de edição
            modoEdicao = true;

            // Desabilita a navegação enquanto estiver editando
            btnAnterior.Enabled = false;
            btnProximo.Enabled = false;
            btnInicio.Enabled = false;
            btnFim.Enabled = false;

            if (!modoEdicao) return; // Só permite incluir se estiver no modo edição

            // Verifica se os campos estão preenchidos
            if (string.IsNullOrWhiteSpace(txtRA.Text) || string.IsNullOrWhiteSpace(txtNome.Text))
            {
                
                MessageBox.Show("Por favor, preencha os campos de palavra e dica.",
                              "Campos vazios",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);     // "Por favor, preencha tanto a palavra quanto a dica.",
                                                           //"Campos vazios",
                return; 
            }

            try
            {
                PalavraDica novaPalavra = new PalavraDica(
                    txtRA.Text.Trim(),
                    txtNome.Text.Trim()
                );

                bool inserido = listaPalavras.InserirEmOrdem(novaPalavra);

                if (inserido)
                {
                    MessageBox.Show("Palavra adicionada com sucesso!",
                                  "Sucesso",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);

                    totalPalavras = listaPalavras.QuantosNos;
                    listaPalavras.PosicionarNoFinal();
                    ExibirRegistroAtual();

                    // Limpa os campos para nova entrada
                    txtRA.Clear();
                    txtNome.Clear();
                    txtRA.Focus();

                    // Volta ao modo normal
                    modoEdicao = false;

                    // Reabilita a navegação
                    btnAnterior.Enabled = true;
                    btnProximo.Enabled = true;
                    btnInicio.Enabled = true;
                    btnFim.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Esta palavra já existe na lista!",
                                  "Palavra duplicada",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao adicionar palavra: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            
        }


    private void btnBuscar_Click(object sender, EventArgs e)
    {
            // se a palavra digitada não é vazia:
            // criar um objeto da classe de Palavra e Dica para busca
            // se a palavra existe na lista1, posicionar o ponteiro atual nesse nó e exibir o registro atual
            // senão, avisar usuário que a palavra não existe
            // exibir o nó atual

            // Verifica se o campo de busca não está vazio
            if (string.IsNullOrWhiteSpace(txtRA.Text))
            {
                MessageBox.Show("Digite uma palavra para buscar.",
                              "Campo vazio",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                txtRA.Focus();
                return;
            }

            try
            {
                // Cria um objeto temporário para busca (usando apenas a palavra)
                PalavraDica palavraBusca = new PalavraDica(txtRA.Text.Trim(), "");

                // Verifica se a palavra existe na lista
                if (listaPalavras.Existe(palavraBusca))
                {
                    // Posiciona no nó encontrado
                    listaPalavras.PosicionarNoInicio();
                    while (listaPalavras.Atual != null &&
                           !listaPalavras.Atual.Info.Palavra.Equals(txtRA.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        listaPalavras.Avancar();
                    }

                    // Atualiza a exibição
                    ExibirRegistroAtual();

                    // Mostra mensagem de encontrado
                    statusStrip1.Items[0].Text = "Palavra encontrada!";
                }
                else
                {
                    MessageBox.Show($"A palavra '{txtRA.Text.Trim()}' não foi encontrada.",
                                  "Não encontrado",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);
                    statusStrip1.Items[0].Text = "Palavra não encontrada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro durante a busca: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }


        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // para o nó atualmente visitado e exibido na tela:
            // perguntar ao usuário se realmente deseja excluir essa palavra e dica
            // se sim, remover o nó atual da lista duplamente ligada e exibir o próximo nó
            // se não, manter como está


            
            // Verifica se há um nó atual válido
            if (listaPalavras.Atual == null || listaPalavras.QuantosNos == 0)
            {
                MessageBox.Show("Não existe palavras para excluir.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            // Pega a palavra atual para exibir na confirmação
            string palavraAtual = listaPalavras.Atual.Info.Palavra;

            // Pergunta ao usuário se deseja realmente excluir
            DialogResult resposta = MessageBox.Show(
                $"Tem certeza que deseja excluir a palavra: {palavraAtual}?",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2); // Default = Não

            if (resposta == DialogResult.Yes)
            {
                try
                {
                    // Guarda a posição atual antes de excluir
                    int posicaoOriginal = posicaoAtual;

                    // Remove o nó atual
                    PalavraDica palavraARemover = listaPalavras.Atual.Info;
                    if (listaPalavras.Remover(palavraARemover))
                    {
                        totalPalavras--;

                        // Atualiza a exibição
                        if (listaPalavras.QuantosNos > 0)
                        {
                            // Se não era o último item, mantém a posição
                            if (posicaoOriginal < totalPalavras)
                            {
                                // Navega até a posição correta
                                listaPalavras.PosicionarNoInicio();
                                for (int i = 0; i < posicaoOriginal; i++)
                                {
                                    listaPalavras.Avancar();
                                }
                            }
                            else
                            {
                                // Se era o último item, vai para o novo último
                                listaPalavras.PosicionarNoFinal();
                                posicaoAtual = totalPalavras - 1;
                            }

                            ExibirRegistroAtual();
                        }
                        else
                        {
                            // Lista ficou vazia
                            txtRA.Clear();
                            txtNome.Clear();
                            posicaoAtual = 0;
                        }

                        // Atualiza o contador
                        slRegistro.Text = $"Registro: {(listaPalavras.QuantosNos > 0 ? (posicaoAtual + 1) : 0)}/{totalPalavras}";

                        MessageBox.Show("Palavra excluída com sucesso!",
                                      "Sucesso",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao excluir palavra: {ex.Message}",
                                  "Erro",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Error);
                }
            }
            
 
        }

    private void FrmAlunos_FormClosing(object sender, FormClosingEventArgs e)
    {
            // solicitar ao usuário que escolha o arquivo de saída
            // percorrer a lista ligada e gravar seus dados no arquivo de saída

            // Só prossegue se houver palavras na lista
            if (listaPalavras == null || listaPalavras.QuantosNos == 0)
                return;

            // Pergunta ao usuário se deseja salvar os dados
            DialogResult resposta = MessageBox.Show(
                "Deseja salvar as palavras e dicas antes de sair?",
                "Salvar dados",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (resposta == DialogResult.Cancel)
            {
                e.Cancel = true; // Cancela o fechamento do formulário
                return;
            }

            if (resposta == DialogResult.Yes)
            {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
                    saveFileDialog.Title = "Salvar palavras e dicas";
                    saveFileDialog.DefaultExt = "txt";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                            {
                                // Dentro do using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                                // Escreve o cabeçalho
                                writer.WriteLine("Palavra com 30 caractere      Dica até o final da linha");

                                // Usar um NoDuplo<Dado> auxiliar em vez de depender do atual da lista
                                NoDuplo<PalavraDica> noAtual = listaPalavras.Primeiro;
                                while (noAtual != null)
                                {
                                    string palavra = noAtual.Info.Palavra.PadRight(30).Substring(0, 30);
                                    string dica = noAtual.Info.Dica;

                                    writer.WriteLine($"{palavra}{dica}");

                                    noAtual = noAtual.Prox; // Avança diretamente sem usar o método Avancar()
                                }
                            }

                            MessageBox.Show("Dados salvos com sucesso!",
                                          "Sucesso",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao salvar arquivo: {ex.Message}",
                                          "Erro",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Error);
                            e.Cancel = true; // Cancela o fechamento para permitir nova tentativa
                        }
                    }
                    else
                    {
                        e.Cancel = true; // Usuário cancelou o save
                    }
                }
            }
        }

    private void ExibirDados(ListaDupla<PalavraDica> aLista, ListBox lsb, Direcao qualDirecao)         //   ListaDupla<Aluno> aLista, ListBox lsb, Direcao qualDirecao
    {
      lsb.Items.Clear();
      var dadosDaLista = aLista.Listagem(qualDirecao);
      foreach (PalavraDica palavraDica in dadosDaLista)
        lsb.Items.Add(palavraDica);


      //foreach (Aluno aluno in dadosDaLista)
        //lsb.Items.Add(aluno);
    }

    private void tabControl1_Enter(object sender, EventArgs e)
    {
      rbFrente.PerformClick();
    }

    private void rbFrente_Click(object sender, EventArgs e)
    {
      ExibirDados(listaPalavras, lsbDados, Direcao.paraFrente);         // ExibirDados(lista1, lsbDados, Direcao.paraFrente);
    }

    private void rbTras_Click(object sender, EventArgs e)
    {
      ExibirDados(listaPalavras, lsbDados, Direcao.paraTras);          // ExibirDados(lista1, lsbDados, Direcao.paraTras);
    }

    private void FrmAlunos_Load(object sender, EventArgs e)
    {
            // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na lista1
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;


            //FazerLeitura(ref listaPalavras);

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
                txtRA.Text = listaPalavras.Atual.Info.Palavra;
                txtNome.Text = listaPalavras.Atual.Info.Dica;
                slRegistro.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";
            }

    }

    private void btnEditar_Click(object sender, EventArgs e)
    {
            // alterar a dica e guardar seu novo valor no nó exibido

            // Verifica se há um nó atual válido
            if (listaPalavras.Atual == null)
            {
                MessageBox.Show("Nenhuma palavra selecionada para editar.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            // Pede confirmação para editar
            DialogResult resposta = MessageBox.Show("Deseja editar a dica desta palavra?",
                                                 "Confirmar Edição",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (resposta == DialogResult.Yes)
            {
                // Habilita apenas o campo de dica para edição
                txtNome.ReadOnly = false;
                txtNome.Focus();
                txtNome.SelectAll(); // Seleciona todo o texto para facilitar a edição

                // Altera o botão Editar para "Salvar"
                btnEditar.Text = "Salvar";
                btnEditar.Click -= btnEditar_Click;
                btnEditar.Click += SalvarEdicao;

                // Desabilita outros botões durante a edição
                btnNovo.Enabled = false;             // btnIncluir
                btnExcluir.Enabled = false;
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;
            }
    }

        private void SalvarEdicao(object sender, EventArgs e)
        {
            try
            {
                // Atualiza apenas a dica no nó atual
                listaPalavras.Atual.Info.Dica = txtNome.Text.Trim();

                MessageBox.Show("Dica atualizada com sucesso!",
                              "Sucesso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao atualizar dica: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
                
                // Restaura o estado original
                txtNome.ReadOnly = true;
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;
                

                // Limpa os campos após edição
                txtRA.Text = "";
                txtNome.Text = "";

                // Restaura o estado original dos controles
                txtNome.ReadOnly = true;
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;

                // Reabilita os botões
                btnNovo.Enabled = true;              // btnIncluir
                btnExcluir.Enabled = true;

                // Só reabilita navegação se houver mais de um registro
                bool podeNavegar = listaPalavras.QuantosNos > 1;
                btnAnterior.Enabled = podeNavegar;
                btnProximo.Enabled = podeNavegar;
                btnInicio.Enabled = podeNavegar;
                btnFim.Enabled = podeNavegar;

                // Atualiza a exibição se ainda houver dados
                if (listaPalavras.QuantosNos > 0)
                {
                    ExibirRegistroAtual();
                }

                // Foco no campo RA para nova operação
                txtRA.Focus();
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            // Sai do modo de edição
            modoEdicao = false;

            // Reabilita os botões de navegação
            btnAnterior.Enabled = (totalPalavras > 1);
            btnProximo.Enabled = (totalPalavras > 1);
            btnInicio.Enabled = (totalPalavras > 1);
            btnFim.Enabled = (totalPalavras > 1);

            // Limpa os campos (opcional - pode comentar esta parte se preferir manter os valores)
            txtRA.Clear();
            txtNome.Clear();

            // Reposiciona e exibe o registro atual (se houver registros)
            if (totalPalavras > 0)
            {
                ExibirRegistroAtual();
            }
            else
            {
                // Se não houver registros, mantém os campos limpos
                txtRA.Text = "";
                txtNome.Text = "";
            }

            // Coloca o foco no campo RA
            txtRA.Focus();
        }
    }

}
