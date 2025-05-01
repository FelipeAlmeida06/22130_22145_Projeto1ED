using System;
using System.Collections.Generic;
using System.Diagnostics;
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


        //private void btnLerArquivo_Click(object sender, EventArgs e)
        //{
        //FazerLeitura(ref listaPalavras);
        //}

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


            if (!modoEdicao)
            {
                // Enable edit mode
                modoEdicao = true;
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;

                txtRA.Clear();
                txtNome.Clear();
                txtRA.Focus();
                return;
            }

            // Validate fields
            if (string.IsNullOrWhiteSpace(txtRA.Text) || string.IsNullOrWhiteSpace(txtNome.Text))
            {
                MessageBox.Show("Por favor, preencha os campos de palavra e dica.",
                              "Campos vazios",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            try
            {
                PalavraDica novaPalavra = new PalavraDica(
                    txtRA.Text.Trim(),
                    txtNome.Text.Trim()
                );

                // Use the fixed version of InserirEmOrdem
                bool inserido = listaPalavras.InserirEmOrdem(novaPalavra);

                if (inserido)
                {
                    // Atualizar arquivo
                    string caminhoArquivo = Path.Combine(Application.StartupPath, "palavras.txt");
                    List<string> linhas;

                    // Check if file exists first
                    if (File.Exists(caminhoArquivo))
                        linhas = File.ReadAllLines(caminhoArquivo).ToList();
                    else
                        linhas = new List<string> { "Palavra com 30 caractere      Dica até o final da linha" };

                    // Format new line properly
                    string novaLinha = $"{novaPalavra.Palavra.PadRight(30).Substring(0, 30)}{novaPalavra.Dica}";

                    // Add the new line after the header if it exists
                    if (linhas.Count > 0 && linhas[0].StartsWith("Palavra com 30 caractere"))
                        linhas.Add(novaLinha); // Add at the end, as order doesn't matter in file
                    else
                        linhas.Add(novaLinha);

                    // Write all lines to file
                    File.WriteAllLines(caminhoArquivo, linhas);

                    MessageBox.Show("Palavra adicionada com sucesso!",
                                  "Sucesso",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);

                    totalPalavras = listaPalavras.QuantosNos;
                    listaPalavras.PosicionarNoFinal();
                    ExibirRegistroAtual();

                    // Reset UI state
                    txtRA.Clear();
                    txtNome.Clear();
                    txtRA.Focus();

                    modoEdicao = false;
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

        private bool buscaEmAndamento = false;
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            // se a palavra digitada não é vazia:
            // criar um objeto da classe de Palavra e Dica para busca
            // se a palavra existe na lista1, posicionar o ponteiro atual nesse nó e exibir o registro atual
            // senão, avisar usuário que a palavra não existe
            // exibir o nó atual


            /*
            if (!modoEdicao)
            {
                // Enable edit mode
                modoEdicao = true;
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;

                txtRA.Clear();
                txtNome.Clear();
                txtRA.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRA.Text))
            {
                MessageBox.Show("Digite uma palavra para buscar.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRA.Focus();
                return;
            }

            try
            {
                string palavraBuscada = txtRA.Text.Trim();
                bool encontrou = false;

                // Salva a posição atual
                int posicaoOriginal = listaPalavras.NumeroDoNoAtual;

                // Começa a busca do início
                listaPalavras.PosicionarNoInicio();

                // Percorre toda a lista
                while (listaPalavras.Atual != null)
                {
                    var palavraAtual = (PalavraDica)listaPalavras.Atual.Info;

                    // Comparação case-insensitive
                    if (palavraAtual.Palavra.Equals(palavraBuscada, StringComparison.OrdinalIgnoreCase))
                    {
                        encontrou = true;
                        break;
                    }

                    listaPalavras.Avancar();
                }

                if (encontrou)
                {
                    var palavraEncontrada = (PalavraDica)listaPalavras.Atual.Info;
                    txtRA.Text = palavraEncontrada.Palavra;
                    txtNome.Text = palavraEncontrada.Dica; // Autocompleta a dica

                    // Atualiza a exibição
                    ExibirRegistroAtual();

                    slRegistro.Text = $"Palavra encontrada: {listaPalavras.NumeroDoNoAtual + 1}/{listaPalavras.QuantosNos}";
                }
                else
                {
                    // Volta para a posição original se não encontrou
                    if (posicaoOriginal >= 0 && posicaoOriginal < listaPalavras.QuantosNos)
                        listaPalavras.PosicionarEm(posicaoOriginal);

                    txtNome.Text = "";
                    MessageBox.Show($"A palavra '{palavraBuscada}' não foi encontrada.",
                                  "Não encontrada",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    slRegistro.Text = "Palavra não encontrada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na busca: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                slRegistro.Text = "Erro durante a busca";
            }
            */


            // Primeiro clique - prepara para busca
            if (!buscaEmAndamento)
            {
                buscaEmAndamento = true;

                // Desabilita navegação durante busca
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;

                // Limpa campos e prepara para nova busca
                txtRA.Clear();
                txtNome.Clear();
                txtRA.Focus();

                // Altera o texto do botão
                btnBuscar.Text = "Buscar";

                return;
            }

            // Segundo clique - executa a busca
            if (string.IsNullOrWhiteSpace(txtRA.Text))
            {
                MessageBox.Show("Digite uma palavra para buscar.", "Aviso",
                               MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRA.Focus();
                return;
            }

            try
            {
                string palavraBuscada = txtRA.Text.Trim();
                bool encontrou = false;

                // Salva a posição atual
                int posicaoOriginal = listaPalavras.NumeroDoNoAtual;

                // Começa a busca do início
                listaPalavras.PosicionarNoInicio();

                // Percorre toda a lista
                while (listaPalavras.Atual != null)
                {
                    var palavraAtual = (PalavraDica)listaPalavras.Atual.Info;

                    // Comparação case-insensitive
                    if (palavraAtual.Palavra.Equals(palavraBuscada, StringComparison.OrdinalIgnoreCase))
                    {
                        encontrou = true;
                        break;
                    }

                    listaPalavras.Avancar();
                }

                if (encontrou)
                {
                    var palavraEncontrada = (PalavraDica)listaPalavras.Atual.Info;
                    txtRA.Text = palavraEncontrada.Palavra;
                    txtNome.Text = palavraEncontrada.Dica; // Autocompleta a dica

                    // Atualiza a exibição
                    ExibirRegistroAtual();

                    //slRegistro.Text = $"Palavra encontrada: {listaPalavras.NumeroDoNoAtual + 1}/{listaPalavras.QuantosNos}";
                    slRegistro.Text = $"Palavra encontrada | Registro: {listaPalavras.NumeroDoNoAtual + 1}/{listaPalavras.QuantosNos}";
                }
                else
                {
                    // Volta para a posição original se não encontrou
                    if (posicaoOriginal >= 0 && posicaoOriginal < listaPalavras.QuantosNos)
                        listaPalavras.PosicionarEm(posicaoOriginal);

                    txtNome.Text = "";
                    MessageBox.Show($"A palavra '{palavraBuscada}' não foi encontrada.",
                                  "Não encontrada",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    slRegistro.Text = "Palavra não encontrada";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na busca: {ex.Message}", "Erro",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
                slRegistro.Text = "Erro durante a busca";
            }
            finally
            {
                // Reseta o estado para permitir nova busca
                buscaEmAndamento = false;

                // Restaura navegação se houver itens
                bool podeNavegar = listaPalavras.QuantosNos > 1;
                btnAnterior.Enabled = podeNavegar;
                btnProximo.Enabled = podeNavegar;
                btnInicio.Enabled = podeNavegar;
                btnFim.Enabled = podeNavegar;

                // Restaura texto do botão
                btnBuscar.Text = "Buscar";
            }
        }


        private void btnExcluir_Click(object sender, EventArgs e)
        {
            // para o nó atualmente visitado e exibido na tela:
            // perguntar ao usuário se realmente deseja excluir essa palavra e dica
            // se sim, remover o nó atual da lista duplamente ligada e exibir o próximo nó
            // se não, manter como está


            if (listaPalavras.Atual == null || listaPalavras.QuantosNos == 0)
            {
                MessageBox.Show("Não existem palavras para excluir.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            string palavraAtual = listaPalavras.Atual.Info.Palavra;

            DialogResult resposta = MessageBox.Show(
                $"Tem certeza que deseja excluir a palavra: {palavraAtual}?",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2);

            if (resposta == DialogResult.Yes)
            {
                try
                {
                    // Guarda a informação antes de remover
                    PalavraDica palavraARemover = listaPalavras.Atual.Info;
                    int posicaoOriginal = posicaoAtual;

                    // Remove da lista em memória
                    if (listaPalavras.Remover(palavraARemover))
                    {
                        totalPalavras--;

                        // Atualiza o arquivo físico IMEDIATAMENTE
                        string caminhoArquivo = Path.Combine(Application.StartupPath, "palavras.txt");
                        string[] linhas = File.ReadAllLines(caminhoArquivo);

                        var novasLinhas = new List<string>();
                        bool cabecalhoMantido = false;

                        for (int i = 0; i < linhas.Length; i++)
                        {
                            // Mantém o cabeçalho
                            if (i == 0 && linhas[i].StartsWith("Palavra com 30 caractere"))
                            {
                                novasLinhas.Add(linhas[i]);
                                cabecalhoMantido = true;
                                continue;
                            }

                            // Verifica se a linha NÃO contém a palavra a ser removida
                            if (i > 0 || !cabecalhoMantido)
                            {
                                string palavraLinha = linhas[i].Length >= 30 ?
                                    linhas[i].Substring(0, 30).Trim() :
                                    linhas[i].Trim();

                                if (!palavraLinha.Equals(palavraARemover.Palavra, StringComparison.OrdinalIgnoreCase))
                                {
                                    novasLinhas.Add(linhas[i]);
                                }
                            }
                        }

                        // Reescreve o arquivo completo
                        File.WriteAllLines(caminhoArquivo, novasLinhas);

                        // Atualiza a exibição
                        if (listaPalavras.QuantosNos > 0)
                        {
                            if (posicaoOriginal >= totalPalavras)
                            {
                                listaPalavras.PosicionarNoFinal();
                                posicaoAtual = totalPalavras - 1;
                            }
                            else
                            {
                                listaPalavras.PosicionarNoInicio();
                                posicaoAtual = 0;
                                for (int i = 0; i < posicaoOriginal && i < totalPalavras; i++)
                                {
                                    listaPalavras.Avancar();
                                    posicaoAtual++;
                                }
                            }
                            ExibirRegistroAtual();
                        }
                        else
                        {
                            txtRA.Clear();
                            txtNome.Clear();
                            posicaoAtual = 0;
                        }

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


            if (listaPalavras == null || listaPalavras.QuantosNos == 0)
                return;

            DialogResult resposta = MessageBox.Show(
                "Deseja salvar as alterações antes de sair?",
                "Salvar dados",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            if (resposta == DialogResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (resposta == DialogResult.Yes)
            {
                string caminhoOriginal = Path.Combine(Application.StartupPath, "palavras.txt");

                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.FileName = "palavras.txt"; // Sugere o mesmo arquivo
                    saveFileDialog.InitialDirectory = Application.StartupPath;
                    saveFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
                    saveFileDialog.Title = "Salvar palavras e dicas";
                    saveFileDialog.DefaultExt = "txt";
                    saveFileDialog.OverwritePrompt = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                            {
                                writer.WriteLine("Palavra com 30 caractere      Dica até o final da linha");

                                NoDuplo<PalavraDica> noAtual = listaPalavras.Primeiro;
                                while (noAtual != null)
                                {
                                    string palavraFormatada = noAtual.Info.Palavra.PadRight(30).Substring(0, 30);
                                    writer.WriteLine($"{palavraFormatada}{noAtual.Info.Dica}");
                                    noAtual = noAtual.Prox;
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
                            e.Cancel = true;
                        }
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
            }
        }

        private void ExibirDados(ListaDupla<PalavraDica> listaPalavras, ListBox lsb, Direcao qualDirecao)         //   ListaDupla<Aluno> aLista, ListBox lsb, Direcao qualDirecao
        {
            //lsb.Items.Clear();
            //var dadosDaLista = listaPalavras.Listagem(qualDirecao);
            //foreach (PalavraDica palavraDica in dadosDaLista)
                //lsb.Items.Add(palavraDica);


            //foreach (Aluno aluno in dadosDaLista)
            //lsb.Items.Add(aluno);


            lsb.Items.Clear();
            var dadosDaLista = listaPalavras.Listagem(qualDirecao);

            foreach (var dado in dadosDaLista)
            {
                if (dado is PalavraDica palavraDica)
                {
                    lsb.Items.Add(palavraDica);
                }
            }

            // Atualiza a mensagem de registro
            slRegistro.Text = $"Registro: {lsb.Items.Count}/{listaPalavras.QuantosNos}";
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

            FazerLeitura(ref listaPalavras);

            try
            {
                // Carrega o arquivo padrão ou pede para o usuário selecionar
                string caminhoPadrao = Path.Combine(Application.StartupPath, "palavras.txt");

                if (File.Exists(caminhoPadrao))
                {
                    // Se existir o arquivo na pasta bin, carrega automaticamente
                    CarregarArquivo(caminhoPadrao);
                }
                else
                {
                    // Se não existir, pede para o usuário selecionar
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";
                        openFileDialog.Title = "Selecione o arquivo de palavras e dicas";

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            CarregarArquivo(openFileDialog.FileName);
                        }
                        else
                        {
                            MessageBox.Show("Nenhum arquivo selecionado. Você pode adicionar palavras manualmente.",
                                        "Atenção",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        }
                    }
                }

                // Posiciona no início e exibe o primeiro registro
                if (listaPalavras.QuantosNos > 0)
                {
                    listaPalavras.PosicionarNoInicio();
                    posicaoAtual = 0;
                    totalPalavras = listaPalavras.QuantosNos;
                    ExibirRegistroAtual();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar arquivo: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }

        }

        private void CarregarArquivo(string caminhoArquivo)
        {
            listaPalavras = new ListaDupla<PalavraDica>();

            string[] linhas = File.ReadAllLines(caminhoArquivo)
                                 .Where(l => !string.IsNullOrWhiteSpace(l))
                                 .ToArray();

            // Ignora o cabeçalho se existir
            int inicio = linhas.Length > 0 && linhas[0].StartsWith("Palavra com 30 caractere") ? 1 : 0;

            for (int i = inicio; i < linhas.Length; i++)
            {
                string linha = linhas[i];
                if (linha.Length >= 30)
                {
                    string palavra = linha.Substring(0, 30).Trim();
                    string dica = linha.Length > 30 ? linha.Substring(30).Trim() : "";

                    if (!string.IsNullOrEmpty(palavra))
                    {
                        listaPalavras.InserirEmOrdem(new PalavraDica(palavra, dica));
                    }
                }
            }
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



            /*
            if (modoEdicao)
                return;

            if (listaPalavras.Atual != null)
            {
                txtRA.Text = listaPalavras.Atual.Info.Palavra;
                txtNome.Text = listaPalavras.Atual.Info.Dica;
                slRegistro.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";
            }
            else if (totalPalavras > 0)
            {
                slRegistro.Text = $"Registro: {totalPalavras}/{totalPalavras}";
            }
            else
            {
                txtRA.Clear();
                txtNome.Clear();
                slRegistro.Text = "Registro: 0/0";
            }
            */


            // Não atualiza durante edição
            if (modoEdicao) return;

            // Atualiza controles com base no estado atual
            if (listaPalavras.Atual != null)
            {
                txtRA.Text = listaPalavras.Atual.Info.Palavra;
                txtNome.Text = listaPalavras.Atual.Info.Dica;
                slRegistro.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";

                // Habilita/desabilita controles de navegação
                btnAnterior.Enabled = (posicaoAtual > 0);
                btnProximo.Enabled = (posicaoAtual < totalPalavras - 1);
                btnInicio.Enabled = (totalPalavras > 1 && posicaoAtual > 0);
                btnFim.Enabled = (totalPalavras > 1 && posicaoAtual < totalPalavras - 1);
            }
            else if (totalPalavras > 0)
            {
                // Caso especial quando atual é null mas há itens
                slRegistro.Text = $"Registro: {totalPalavras}/{totalPalavras}";
            }
            else
            {
                // Lista vazia
                txtRA.Clear();
                txtNome.Clear();
                slRegistro.Text = "Registro: 0/0";

                // Desabilita todos os botões de navegação
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;
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
            /*
            // Sai do modo de edição
            modoEdicao = false;

            // Reabilita os botões de navegação
            btnAnterior.Enabled = (totalPalavras > 1);
            btnProximo.Enabled = (totalPalavras > 1);
            btnInicio.Enabled = (totalPalavras > 1);
            btnFim.Enabled = (totalPalavras > 1);

            // Limpa os campos
            txtRA.Clear();
            txtNome.Clear();

            // Reposiciona e exibe o registro atual (se houver registros)
            if (totalPalavras > 0)
            {
                ExibirRegistroAtual();
            }

            // Coloca o foco no campo RA
            txtRA.Focus();
            */


            if (modoEdicao)
            {
                modoEdicao = false;

                // Restaura controles de edição
                txtRA.ReadOnly = true;
                txtNome.ReadOnly = true;

                // Restaura botão Editar
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;

                // Reabilita controles
                btnNovo.Enabled = true;
                btnExcluir.Enabled = listaPalavras.QuantosNos > 0;
                btnCancelar.Enabled = false;

                // Força atualização após cancelar edição
                ExibirRegistroAtual();
            }

            // Coloca o foco no campo RA
            txtRA.Focus();
        }
    }

}
