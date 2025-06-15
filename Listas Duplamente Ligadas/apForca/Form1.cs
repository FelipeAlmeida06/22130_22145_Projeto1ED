// Nome: Felipe Antônio de Oliveira Almeida     RA: 22130
// Nome: Miguel de Castro Chaga Silva           RA: 22145

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using static System.Resources.ResXFileRef;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace apListaLigada
{
    public partial class FrmAlunos : Form
    {
        private ListaDupla<PalavraDica> listaPalavras;
        private int posicaoAtual = 0;
        private int totalPalavras = 0;

        public FrmAlunos()
        {
            InitializeComponent();
            listaPalavras = new ListaDupla<PalavraDica>();

            // Jogo da Forca
            dicio = new VetorDicionario(100);
            ConfigurarDataGridView();

            botoesAlfabeto = new List<Button>(); // Inicializa a lista de botões
        }


        //private void btnLerArquivo1_Click(object sender, EventArgs e)
        //{
        //        //FazerLeitura(ref listaPalavras);       // FazerLeitura(ref lista1);
        //}


        private void FazerLeitura(ref ListaDupla<PalavraDica> qualLista)
        {
            // instanciar a lista de palavras e dicas
            // pedir ao usuário o nome do arquivo de entrada
            // abrir esse arquivo e lê-lo linha a linha
            // para cada linha, criar um objeto da classe de Palavra e Dica
            // e inseri-0lo no final da lista duplamente ligada

           
            qualLista = new ListaDupla<PalavraDica>(); // inicializa uma nova lista duplamente ligada de PalavraDica

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";   // filtra arquivos textos
                openFileDialog.Title = "Selecione o arquivo de palavras e dicas";  // titulo da janela de diálogo

                if (openFileDialog.ShowDialog() == DialogResult.OK)  // se o usuário selecionar um arquivo e confirmar
                {
                    try
                    {
                        // lê todas as linhas do arquivo e remove linhas em branco
                        string[] linhas = File.ReadAllLines(openFileDialog.FileName)
                                              .Where(l => !string.IsNullOrWhiteSpace(l))
                                              .ToArray();

                        for (int i = 1; i < linhas.Length; i++)
                        {
                            string linha = linhas[i];

                            if (linha.Length >= 30)
                            {
                                // extrai os primeiros 30 caracteres como palavra e o resto como dica
                                string palavra = linha.Substring(0, 30).Trim();
                                string dica = linha.Substring(30).Trim();

                                // verifica se a palavra não está vazia
                                if (!string.IsNullOrWhiteSpace(palavra))
                                {
                                    PalavraDica nova = new PalavraDica(palavra, dica);  // cria um novo objeto PalavraDica com os dados da linha
                                    qualLista.InserirAposFim(nova);  // insere o objeto no final da lista
                                }
                            }
                            else
                            {
                                // não faz nada
                            }
                        }
                        
                        // atualiza a variável 'totalPalavras' com o número de nós da lista
                        totalPalavras = qualLista.QuantosNos;
                        if (totalPalavras > 0)
                        {
                            qualLista.PosicionarNoInicio();  // posiciona o ponteiro da lista no início
                            ExibirRegistroAtual();  // exibe o registro atual (primeiro elemento do arquivo texto)
                        }
                        else   // mensagem de aviso
                        {
                            MessageBox.Show("O arquivo selecionado não contém palavras válidas.", "Aviso",
                                          MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    catch (Exception ex)    // mensagem de erro
                    {
                        MessageBox.Show($"Erro ao processar arquivo: {ex.Message}", "Erro",
                                      MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else    // mensagem de aviso
                {
                    MessageBox.Show("Nenhum arquivo foi selecionado. Por favor, selecione um arquivo para continuar.",
                                  "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }    
        }

        private bool modoEdicao = false;  // variável para controlar o estado

        private void btnIncluir_Click(object sender, EventArgs e)
        {
            // se o usuário digitou palavra e dica:
            // criar objeto da classe Palavra e Dica para busca
            // tentar incluir em ordem esse objeto na lista1
            // se não incluiu (já existe) avisar o usuário

            if (!modoEdicao)
            {

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

            if (string.IsNullOrWhiteSpace(txtRA.Text) || string.IsNullOrWhiteSpace(txtNome.Text))   // se os dois campos estiverem em branco (vazios)
            {
                // exibe mensagem de aviso
                MessageBox.Show("Por favor, preencha os campos de palavra e dica.",
                              "Campos vazios",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // cria um objeto da classe PalavraDica, passando dois valores como parâmetros para o construtor da classe
                PalavraDica novaPalavra = new PalavraDica(
                    txtRA.Text.Trim(),    // campo Palavra
                    txtNome.Text.Trim()   // campo Dica
                );

                // tenta inserir a nova palavra na lista em ordem
                bool inserido = listaPalavras.InserirEmOrdem(novaPalavra);

                if (inserido)   // se a inserção foi bem-sucedida
                {
                    // atualizar arquivo
                    string caminhoArquivo = Path.Combine(Application.StartupPath, "palavras.txt");
                    List<string> linhas;

                    // verifica se o arquivo já existe
                    if (File.Exists(caminhoArquivo))
                        linhas = File.ReadAllLines(caminhoArquivo).ToList(); // lê todas as linhas do arquivo e converte para uma lista
                    else
                        linhas = new List<string> { "Palavra com 30 caractere      Dica até o final da linha" };


                    // cria uma nova linha no formato exigido: palavra com 30 caracteres + dica
                    string novaLinha = $"{novaPalavra.Palavra.PadRight(30).Substring(0, 30)}{novaPalavra.Dica}";

                    // se a primeira linha for o cabeçalho esperado, adiciona a nova linha no final
                    if (linhas.Count > 0 && linhas[0].StartsWith("Palavra com 30 caractere"))
                        linhas.Add(novaLinha);
                    else
                        linhas.Add(novaLinha);  // adiciona a nova linha

                    // escreve todas as linhas atualizadas de volta no arquivo
                    File.WriteAllLines(caminhoArquivo, linhas);

                    // a palavra foi adicionada com sucesso
                    MessageBox.Show("Palavra adicionada com sucesso!",
                                  "Sucesso",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Information);

                    totalPalavras = listaPalavras.QuantosNos;  // atualiza o total de palavras na lista
                    listaPalavras.PosicionarNoFinal();  // último elemento inserido
                    ExibirRegistroAtual();  // exibe o conteúdo do item atual

                    // limpa os campos de texto
                    txtRA.Clear();
                    txtNome.Clear();
                    txtRA.Focus();

                    modoEdicao = false;
                    btnAnterior.Enabled = true;
                    btnProximo.Enabled = true;
                    btnInicio.Enabled = true;
                    btnFim.Enabled = true;
                }
                else   // mensagem de aviso
                {
                    MessageBox.Show("Esta palavra já existe na lista!",
                                  "Palavra duplicada",
                                  MessageBoxButtons.OK,
                                  MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)   // mensagem de erro
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

            // primeiro clique - prepara para busca
            if (!buscaEmAndamento)
            {
                buscaEmAndamento = true;

                // desabilita navegação durante busca
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;

                // limpa campos e prepara para nova busca
                txtRA.Clear();
                txtNome.Clear();
                txtRA.Focus();

                // altera o texto do botão
                btnBuscar.Text = "Buscar";

                return;
            }

            // segundo clique - executa a busca
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

                // salva a posição atual
                int posicaoOriginal = listaPalavras.NumeroDoNoAtual;

                // começa a busca do início
                listaPalavras.PosicionarNoInicio();

                // percorre toda a lista
                while (listaPalavras.Atual != null)
                {
                    var palavraAtual = (PalavraDica)listaPalavras.Atual.Info;

                    // comparação case-insensitive (caracteres maiúsculas ou minúsculas)
                    if (palavraAtual.Palavra.Equals(palavraBuscada, StringComparison.OrdinalIgnoreCase))
                    {
                        encontrou = true;
                        break;
                    }

                    listaPalavras.Avancar();   // avança na navegação
                }

                if (encontrou)
                {
                    var palavraEncontrada = (PalavraDica)listaPalavras.Atual.Info;
                    txtRA.Text = palavraEncontrada.Palavra;
                    txtNome.Text = palavraEncontrada.Dica; // autocompleta a dica

                    ExibirRegistroAtual();   // exibe o registro atual

                    slRegistro.Text = $"Palavra encontrada | Registro: {listaPalavras.NumeroDoNoAtual + 1}/{listaPalavras.QuantosNos}";
                }
                else
                {
                    // volta para a posição original se não encontrou
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
                // reseta o estado para permitir nova busca
                buscaEmAndamento = false;

                // restaura navegação se houver itens
                bool podeNavegar = listaPalavras.QuantosNos > 1;
                btnAnterior.Enabled = podeNavegar;
                btnProximo.Enabled = podeNavegar;
                btnInicio.Enabled = podeNavegar;
                btnFim.Enabled = podeNavegar;

                // restaura texto do botão
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

            string palavraAtual = listaPalavras.Atual.Info.Palavra; // acessa a palavra do nó atual da lista

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
                    // guarda a informação antes de remover
                    PalavraDica palavraARemover = listaPalavras.Atual.Info;
                    int posicaoOriginal = posicaoAtual;

                    // remove da lista em memória
                    if (listaPalavras.Remover(palavraARemover))
                    {
                        totalPalavras--;   // atualiza o total de palavras na lista

                        string caminhoArquivo = Path.Combine(Application.StartupPath, "palavras.txt");  // arquivo de entrada
                        string[] linhas = File.ReadAllLines(caminhoArquivo);  // lê todas as linhas do arquivo

                        var novasLinhas = new List<string>();  // lista para armazenar as linhas do arquivo
                        bool cabecalhoMantido = false;  // manter o cabeçalho do arquivo

                        for (int i = 0; i < linhas.Length; i++)
                        {
                            // mantém o cabeçalho
                            if (i == 0 && linhas[i].StartsWith("Palavra com 30 caractere"))
                            {
                                novasLinhas.Add(linhas[i]);
                                cabecalhoMantido = true;
                                continue; // pula para a próxima linha
                            }

                            // verifica se a linha NÃO contém a palavra a ser removida
                            if (i > 0 || !cabecalhoMantido)
                            {
                                // extrai a palavra da linha (primeiros 30 caracteres ou menos)
                                string palavraLinha = linhas[i].Length >= 30 ?
                                    linhas[i].Substring(0, 30).Trim() :
                                    linhas[i].Trim();

                                // compara ignorando case-sensitive. Se for diferente, mantém a linha
                                if (!palavraLinha.Equals(palavraARemover.Palavra, StringComparison.OrdinalIgnoreCase))
                                {
                                    novasLinhas.Add(linhas[i]);
                                }
                            }
                        }

                        File.WriteAllLines(caminhoArquivo, novasLinhas);  // reescreve o arquivo completo

                        if (listaPalavras.QuantosNos > 0)  // atualiza a exibição
                        {
                            if (posicaoOriginal >= totalPalavras) // se excluiu a última palavra
                            {
                                listaPalavras.PosicionarNoFinal();  // vai para a nova última palavra
                                posicaoAtual = totalPalavras - 1;
                            }
                            else
                            {
                                listaPalavras.PosicionarNoInicio();  // vai para o início e avança até a posição original
                                posicaoAtual = 0;
                                for (int i = 0; i < posicaoOriginal && i < totalPalavras; i++)
                                {
                                    listaPalavras.Avancar();  // avançar um nó
                                    posicaoAtual++;
                                }
                            }
                            ExibirRegistroAtual(); // atualiza a exibição com a nova palavra atual
                        }
                        else
                        {
                            txtRA.Clear();   // limpa o campo 
                            txtNome.Clear(); // limpa o campo
                            posicaoAtual = 0;   // zera a posição
                        }

                        // atualiza o rótulo com a posição atual e total de registros
                        slRegistro.Text = $"Registro: {(listaPalavras.QuantosNos > 0 ? (posicaoAtual + 1) : 0)}/{totalPalavras}";
                        MessageBox.Show("Palavra excluída com sucesso!",
                                      "Sucesso",
                                      MessageBoxButtons.OK,
                                      MessageBoxIcon.Information);// a palavra foi excluida com sucesso
                    }
                }
                catch (Exception ex)    // mensagem de erro
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

            // não há nada para salvar
            if (listaPalavras == null || listaPalavras.QuantosNos == 0)
                return;

            // pergunta ao usuário se ele deseja salvar as alterações antes de sair
            DialogResult resposta = MessageBox.Show(
                "Deseja salvar as alterações antes de sair?",
                "Salvar dados",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question);

            // se o usuário clicar em "Cancelar"
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
                    saveFileDialog.FileName = "palavras.txt"; // arquivo de entrada
                    saveFileDialog.InitialDirectory = Application.StartupPath; ; // diretório padrão
                    saveFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt"; // filtro de extensão
                    saveFileDialog.Title = "Salvar palavras e dicas"; // título da janela
                    saveFileDialog.DefaultExt = "txt"; // extensão padrão
                    saveFileDialog.OverwritePrompt = true;

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            // abre o arquivo para escrita
                            using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                            {
                                writer.WriteLine("Palavra com 30 caractere      Dica até o final da linha");

                                // começa do primeiro nó da lista
                                NoDuplo<PalavraDica> noAtual = listaPalavras.Primeiro;
                                while (noAtual != null)// percorre a lista
                                {
                                    string palavraFormatada = noAtual.Info.Palavra.PadRight(30).Substring(0, 30);
                                    writer.WriteLine($"{palavraFormatada}{noAtual.Info.Dica}");  // escreve a linha no arquivo com a palavra formatada + dica
                                    noAtual = noAtual.Prox;  // avança para o próximo nó
                                }
                            }

                            MessageBox.Show("Dados salvos com sucesso!",
                                          "Sucesso",
                                          MessageBoxButtons.OK,
                                          MessageBoxIcon.Information);    // dados foram salvos com sucesso
                        }
                        catch (Exception ex)  // mensagem de erro
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
                        e.Cancel = true;  // se o usuário cancelar o diálogo de salvar, cancela o fechamento do formulário
                    }
                }
            }
        }

        private void ExibirDados(ListaDupla<PalavraDica> listaPalavras, ListBox lsb, Direcao qualDirecao)         //   ListaDupla<Aluno> aLista, ListBox lsb, Direcao qualDirecao
        {
            lsb.Items.Clear();   // limpa todos os itens atuais do ListBox antes de exibir novos
            var dadosDaLista = listaPalavras.Listagem(qualDirecao);   // obtém os dados da lista na direção (para frente ou para trás)

            foreach (var dado in dadosDaLista) // percorre todos os dados obtidos
            {
                if (dado is PalavraDica palavraDica) // verifica se o objeto é do tipo PalavraDica
                {
                    lsb.Items.Add(palavraDica);// adiciona o objeto ao ListBox
                }
            }

            slRegistro.Text = $"Registro: {lsb.Items.Count}/{listaPalavras.QuantosNos}";  // atualiza a mensagem de registro
        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {
            rbFrente.PerformClick();
        }

        private void rbFrente_Click(object sender, EventArgs e)
        {
            ExibirDados(listaPalavras, lsbDados, Direcao.paraFrente);
        }

        private void rbTras_Click(object sender, EventArgs e)
        {
            ExibirDados(listaPalavras, lsbDados, Direcao.paraTras);
        }

        private void FrmAlunos_Load(object sender, EventArgs e)
        {
            /*  VERSÃO ANTERIOR: ABA CADASTRO E LISTA
            // fazer a leitura do arquivo escolhido pelo usuário e armazená-lo na lista1
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            FazerLeitura(ref listaPalavras);  // realiza a leitura inicial do arquivo e carrega na listaPalavras

            try
            {
                // carrega o arquivo padrão ou pede para o usuário selecionar
                string caminhoPadrao = Path.Combine(Application.StartupPath, "palavras.txt");

                if (File.Exists(caminhoPadrao))
                {
                    // carrega o conteúdo do arquivo padrão
                    CarregarArquivo(caminhoPadrao);
                }
                else
                {
                    // se não existir, pede para o usuário selecionar
                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                    {
                        openFileDialog.Filter = "Arquivos de texto (*.txt)|*.txt";     // filtra arquivos texto
                        openFileDialog.Title = "Selecione o arquivo de palavras e dicas";  // titulo da janela de diálogo

                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            CarregarArquivo(openFileDialog.FileName); // carrega o arquivo escolhido
                        }
                        else  // mensagem de aviso, nenhum arquivo selecionado
                        {
                            MessageBox.Show("Nenhum arquivo selecionado. Você pode adicionar palavras manualmente.",
                                        "Atenção",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information);
                        }
                    }
                }

                // posiciona no início e exibe o primeiro registro
                if (listaPalavras.QuantosNos > 0)
                {
                    listaPalavras.PosicionarNoInicio();// move o ponteiro atual para o primeiro nó da lista
                    posicaoAtual = 0;// define que a posição atual é a primeira
                    totalPalavras = listaPalavras.QuantosNos;// atualiza o total de palavras
                    ExibirRegistroAtual();// mostra o primeiro item na interface
                }
            }
            catch (Exception ex)    // mensagem de erro
            {
                MessageBox.Show($"Erro ao carregar arquivo: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            */


            //NOVA VERSÃO: ABA FORCA
            dicio = new VetorDicionario(5000);

            CarregarDadosDoArquivoJogoDaForca();

            dicio.PosicionarNoPrimeiro();
            AtualizarTela(); // Atualiza todos os campos e o DataGridView

            ConfigurarDataGridView(); // Configura dgvPalavrasEDicas

            // Adiciona todos os botões do alfabeto à lista.
            AdicionarBotoesAlfabeto(btnA, btnB, btnC, btnD, btnE, btnF, btnG, btnH, btnI, btnJ, btnK, btnL, btnM,
                                    btnN, btnO, btnP, btnQ, btnR, btnS, btnT, btnU, btnV, btnW, btnX, btnY, btnZ,
                                    btnÇ, btnÁ, btnÂ, btnÃ, btnÉ, btnÊ, btnÍ, btnÓ, btnÔ, btnÕ, btnÚ,
                                    btnHifen, btnEspaco);

            //Desabilita os botões do teclado no início do formulário
            foreach (Button btn in botoesAlfabeto)
            {
                btn.Enabled = false;
            }

            btnIniciar.Enabled = true;
            txtNome.Enabled = true;
            chkComDica.Enabled = true;

            //dgvPalavra
            if (dgvPalavra.Columns.Count == 0)
            {
                dgvPalavra.Columns.Add("Placeholder", "");
            }
            dgvPalavra.ReadOnly = true;
            dgvPalavra.AllowUserToAddRows = false;
            dgvPalavra.AllowUserToDeleteRows = false;
            dgvPalavra.AllowUserToResizeColumns = false;
            dgvPalavra.AllowUserToResizeRows = false;
            dgvPalavra.ColumnHeadersVisible = false;
            dgvPalavra.RowHeadersVisible = false;
            dgvPalavra.ScrollBars = ScrollBars.None;
            dgvPalavra.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvPalavra.DefaultCellStyle.SelectionBackColor = dgvPalavra.DefaultCellStyle.BackColor;
            dgvPalavra.DefaultCellStyle.SelectionForeColor = dgvPalavra.DefaultCellStyle.ForeColor;
            dgvPalavra.BackgroundColor = System.Drawing.Color.LightGray;


            ComputarImagensBonecoEnforcado(false);
            BonecoEstaVivo(false);
            ComputarImagensDaForca(true);
            picBoxEspiritoBoneco.Visible = false;

            tmrBarraDeStatus.Enabled = true; // // Garante que o timer esteja habilitado
            tmrForca.Interval = 1000;  // Configura o intervalo do timer da forca para 1 segundo (1000 milissegundos)
        }

        private void CarregarArquivo(string caminhoArquivo)
        {
            listaPalavras = new ListaDupla<PalavraDica>();// cria nova lista

            // lê todas as linhas do arquivo, ignorando linhas em branco
            string[] linhas = File.ReadAllLines(caminhoArquivo)
                                 .Where(l => !string.IsNullOrWhiteSpace(l))
                                 .ToArray();

            // ignora o cabeçalho se existir
            int inicio = linhas.Length > 0 && linhas[0].StartsWith("Palavra com 30 caractere") ? 1 : 0;

            for (int i = inicio; i < linhas.Length; i++) // percorre cada linha do arquivo
            {
                string linha = linhas[i];
                if (linha.Length >= 30)
                {
                    // extrai a palavra (30 primeiros caracteres) e a dica (restante da linha)
                    string palavra = linha.Substring(0, 30).Trim();
                    string dica = linha.Length > 30 ? linha.Substring(30).Trim() : "";

                    if (!string.IsNullOrEmpty(palavra)) // se a palavra não estiver vazia
                    {
                        // cria novo objeto PalavraDica e insere ordenadamente na lista
                        listaPalavras.InserirEmOrdem(new PalavraDica(palavra, dica));
                    }
                }
            }
        }

        private void btnInicio_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no início da lista duplamente ligada
            // Exibir o Registro Atual;

            //listaPalavras.PosicionarNoInicio();
            //posicaoAtual = 0;
            //ExibirRegistroAtual();


            //NOVA VERSÃO: JOGO DA FORCA
            dicio.PosicionarNoPrimeiro();
            AtualizarTela();
        }

        private void btnAnterior_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó imediatamente anterior 
            // Exibir o Registro Atual;

            //listaPalavras.Retroceder();
            //if (posicaoAtual > 0) posicaoAtual--;
            //ExibirRegistroAtual();


            //NOVA VERSÃO: JOGO DA FORCA
            dicio.RetrocederPosicao();
            AtualizarTela();
        }

        private void btnProximo_Click(object sender, EventArgs e)
        {
            // Retroceder o ponteiro atual para o nó seguinte 
            // Exibir o Registro Atual;

            //listaPalavras.Avancar();
            //if (posicaoAtual < totalPalavras - 1) posicaoAtual++;
            //ExibirRegistroAtual();


            //NOVA VERSÃO: JOGO DA FORCA
            dicio.AvancarPosicao();
            AtualizarTela();
        }

        private void btnFim_Click(object sender, EventArgs e)
        {
            // posicionar o ponteiro atual no último nó da lista 
            // Exibir o Registro Atual;

            //listaPalavras.PosicionarNoFinal();
            //posicaoAtual = totalPalavras - 1;
            //ExibirRegistroAtual();


            //NOVA VERSÃO: JOGO DA FORCA
            dicio.PosicionarNoUltimo();
            AtualizarTela();
        }

        private void ExibirRegistroAtual()
        {
            // se a lista não está vazia:
            // acessar o nó atual e exibir seus campos em txtDica e txtPalavra
            // atualizar no status bar o número do registro atual / quantos nós na lista

            // não atualiza durante edição
            if (modoEdicao) return;

            // atualiza controles com base no estado atual
            if (listaPalavras.Atual != null)
            {
                txtRA.Text = listaPalavras.Atual.Info.Palavra; // mostra a palavra no txt
                txtNome.Text = listaPalavras.Atual.Info.Dica;// mostra a dica no txt
                // atualiza a barra de status com o número do registro atual e total de registros
                slRegistro.Text = $"Registro: {posicaoAtual + 1}/{totalPalavras}";

                // habilita/desabilita controles de navegação
                btnAnterior.Enabled = (posicaoAtual > 0);// só habilita "Anterior" se não estiver no início
                btnProximo.Enabled = (posicaoAtual < totalPalavras - 1);// só habilita "Próximo" se não estiver no fim
                btnInicio.Enabled = (totalPalavras > 1 && posicaoAtual > 0);// só habilita "Início" se houver mais de 1 item e não estiver no primeiro
                btnFim.Enabled = (totalPalavras > 1 && posicaoAtual < totalPalavras - 1);// só habilita "Fim" se houver mais de 1 item e não estiver no último
            }
            else if (totalPalavras > 0) // caso a lista tenha itens, mas o ponteiro atual esteja nulo
            {
                slRegistro.Text = $"Registro: {totalPalavras}/{totalPalavras}";// mostra o total de registros
            }
            else
            {
                // lista está vazia
                txtRA.Clear();
                txtNome.Clear();
                slRegistro.Text = "Registro: 0/0";

                // desabilita todos os botões de navegação
                btnAnterior.Enabled = false;
                btnProximo.Enabled = false;
                btnInicio.Enabled = false;
                btnFim.Enabled = false;
            }
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            // alterar a dica e guardar seu novo valor no nó exibido

            // verifica se há um nó atual válido
            if (listaPalavras.Atual == null)
            {
                MessageBox.Show("Nenhuma palavra selecionada para editar.",
                              "Aviso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);
                return;
            }

            // pede confirmação para editar
            DialogResult resposta = MessageBox.Show("Deseja editar a dica desta palavra?",
                                                 "Confirmar Edição",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (resposta == DialogResult.Yes)
            {
                // habilita apenas o campo de dica para edição
                txtNome.ReadOnly = false;
                txtNome.Focus();
                txtNome.SelectAll();

                // altera o botão Editar para "Salvar"
                btnEditar.Text = "Salvar";
                btnEditar.Click -= btnEditar_Click;
                btnEditar.Click += SalvarEdicao;

                // desabilita outros botões durante a edição
                btnNovo.Enabled = false;
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
                // atualiza apenas a dica no nó atual
                listaPalavras.Atual.Info.Dica = txtNome.Text.Trim();

                MessageBox.Show("Dica atualizada com sucesso!",
                              "Sucesso",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Information);   // a dica foi atualizada com sucesso
            }
            catch (Exception ex)   // mensagem de erro
            {
                MessageBox.Show($"Erro ao atualizar dica: {ex.Message}",
                              "Erro",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
            finally
            {
                // restaura o estado original
                txtNome.ReadOnly = true;
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;

                // limpa os campos após edição
                txtRA.Text = "";
                txtNome.Text = "";

                // restaura o estado original dos controles
                txtNome.ReadOnly = true;
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;

                // reabilita os botões
                btnNovo.Enabled = true;
                btnExcluir.Enabled = true;

                // só reabilita navegação se houver mais de um registro
                bool podeNavegar = listaPalavras.QuantosNos > 1;
                btnAnterior.Enabled = podeNavegar;
                btnProximo.Enabled = podeNavegar;
                btnInicio.Enabled = podeNavegar;
                btnFim.Enabled = podeNavegar;

                // atualiza a exibição se ainda houver dados
                if (listaPalavras.QuantosNos > 0)
                {
                    ExibirRegistroAtual();
                }

                txtRA.Focus();
            }
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if (modoEdicao)
            {
                modoEdicao = false;

                // restaura controles de edição
                txtRA.ReadOnly = true;
                txtNome.ReadOnly = true;

                // restaura botão Editar
                btnEditar.Text = "Editar";
                btnEditar.Click -= SalvarEdicao;
                btnEditar.Click += btnEditar_Click;

                // reabilita controles
                btnNovo.Enabled = true;
                btnExcluir.Enabled = listaPalavras.QuantosNos > 0;
                btnCancelar.Enabled = false;

                ExibirRegistroAtual();
            }

            txtRA.Focus();
        }


        // =========================== PROJETO DE FORCA ===========================
        // Variáveis para a parte do jogo de forca
        VetorDicionario dicio;
        int indiceAleatorio;
        int tempoForca;
        int pontos = 0;
        int erros = 0;

        bool emJogo;
        string inspiracao = "http://www.velhosamigos.com.br/jogos/forca.htm";
        string nome;

        private string palavraSecreta; // A palavra que o jogador deve adivinhar
        private StringBuilder palavraOcultaDisplay; // A palavra com underlines, que será mostrada ao jogador

        private const int MAX_ERROS_VISUAIS = 8; // Número de partes progressivas do boneco (1 a 8)
        private const int ERRO_TROCA_ROSTO = 9;   // O erro onde a imagem do rosto muda para XX
        private const int LIMITE_ERROS_PARA_PERDER = 9; // O jogo termina no erro 9 (com a troca de rosto e o espírito)

        // Botões do alfabeto (certifique-se de que todos esses nomes existem no designer)
        private List<Button> botoesAlfabeto;
        private void IniciarNovoJogoForca()
        {
            if (dicio == null || dicio.EstaVazio)
            {
                MessageBox.Show("Não há palavras carregadas para iniciar o jogo da Forca. Por favor, carregue um arquivo.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnIniciar.Enabled = true; // Permite tentar iniciar novamente
                return;
            }
            pontos = 0;
            erros = 0; // Zera o contador de erros
            emJogo = true; // Define o estado do jogo como "em andamento"

            Random rand = new Random();
            indiceAleatorio = rand.Next(0, dicio.Tamanho);
            Dicionario entradaSelecionada = dicio[indiceAleatorio];

            palavraSecreta = entradaSelecionada.Palavra.ToUpper().Trim(); // Converte para maiúsculas e remove espaços

            palavraOcultaDisplay = new StringBuilder();
            dgvPalavra.Columns.Clear();
            dgvPalavra.Rows.Clear();

            for (int i = 0; i < palavraSecreta.Length; i++)
            {
                dgvPalavra.Columns.Add($"col{i}", ""); // Nome da coluna e texto do cabeçalho vazio
                dgvPalavra.Columns[i].Width = 31; // Define largura da coluna
                dgvPalavra.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; // Centraliza o texto

                char charOriginal = palavraSecreta[i];
                if (char.IsLetter(charOriginal))
                {
                    palavraOcultaDisplay.Append("_");
                }
                else // Mantém caracteres que não são letras (espaços, hífens, etc.)
                {
                    palavraOcultaDisplay.Append(charOriginal);
                }
            }

            AtualizarDisplayPalavraOculta();

            // Atualizar os Labels de status
            lbPontos.Text = $"Pontos: {pontos}";
            lbErros.Text = $"Erros: {erros}"; // Mostra erros/total de partes visíveis
            lbDica.Text = "Dica: " + entradaSelecionada.Dica;

            ResetarImagens(); // Seu método para resetar a visibilidade de todos os PictureBoxes
            AtualizarImagemForca(); // Exibe a imagem inicial (forca vazia, ou a primeira parte se 'erros' já fosse 1)

            ReabilitarBotoesAlfabeto();

            if (chkComDica.Checked)
            {
                tempoForca = 60; // Seta o tempo para 60 segundos
                tmrForca.Enabled = true; // Habilita o timer
                lbDica.Visible = true; // Mostra a dica
                lbTempo.Visible = true; // Mostra o label de tempo
                lbTempo.Text = $"Tempo Restante: {tempoForca}s";
            }
            else
            {
                tempoForca = 0; // Desabilita o tempo se não tiver dica
                tmrForca.Enabled = false;
                lbDica.Visible = false; // Esconde a dica
                lbTempo.Visible = false; // Esconde o label de tempo
            }


            // Desabilitar/habilitar controles durante o jogo
            btnIniciar.Enabled = false; // Botão Iniciar desabilitado
            txtNome.Enabled = false; // Nome do jogador desabilitado
            chkComDica.Enabled = false; // Checkbox de dica desabilitado

            // Habilita o botão Jogar Novamente quando o jogo começa
            btnJogarNovamente.Enabled = true;
        }

        private void btnIniciar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textNome.Text))
            {
                MessageBox.Show("Por favor, insira um nome válido antes de começar a jogar.", "Aviso: Nome inválido ou não inserido pelo usuário", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNome.Focus();
                return;
            }

            IniciarNovoJogoForca();
        }

        private void tpCadastro_Enter(object sender, EventArgs e)
        {

        }

        private void CarregarDadosDoArquivoJogoDaForca()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Selecione o arquivo de palavras e dicas";
            openFileDialog.Filter = "Arquivos de Texto (*.txt)|*.txt|Todos os Arquivos (*.*)|*.*";
            openFileDialog.InitialDirectory = Application.StartupPath; // Começa no diretório do executável

            // Exibe a caixa de diálogo e verifica se o usuário selecionou um arquivo
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string caminhoDoArquivo = openFileDialog.FileName;

                try
                {
                    // Limpa o dicio antes de carregar novos dados
                    // (Você pode adicionar um método Clear() ou Reset() em VetorDicionario)
                    // Por enquanto, vamos recriar a instância para garantir que esteja vazio.
                    dicio = new VetorDicionario(5000); // Recria para zerar os dados

                    using (StreamReader arq = new StreamReader(caminhoDoArquivo))
                    {
                        while (!arq.EndOfStream)
                        {
                            Dicionario novaEntrada = new Dicionario();
                            novaEntrada.LerDados(arq); // Lê uma linha do arquivo e popula o objeto Dicionario
                            dicio.Incluir(novaEntrada); // Inclui o objeto Dicionario no VetorDicionario
                        }
                    }

                    // Agora que os dados foram carregados no dicio, exiba-os no DataGridView
                    dicio.ExibirDados(dgvPalavrasEDicas);
                }
                catch (FileNotFoundException)
                {
                    MessageBox.Show("O arquivo não foi encontrado.", "Erro de Arquivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    //MessageBox.Show($"Ocorreu um erro ao carregar o arquivo: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Nenhum arquivo selecionado. O programa pode não funcionar corretamente sem os dados.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ConfigurarDataGridView()
        {
            dgvPalavrasEDicas.AutoGenerateColumns = false;

            // Adiciona as colunas se elas ainda não existirem
            if (dgvPalavrasEDicas.Columns.Count == 0)
            {
                dgvPalavrasEDicas.Columns.Add("Palavra", "Palavra");
                dgvPalavrasEDicas.Columns.Add("Dica", "Dica");
            }
            dgvPalavrasEDicas.Columns["Palavra"].Width = 150;
            dgvPalavrasEDicas.Columns["Dica"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void AtualizarDisplayPalavraOculta()
        {
            // Limpa as linhas e colunas existentes
            dgvPalavra.Rows.Clear();
            dgvPalavra.Columns.Clear();

            for (int i = 0; i < palavraOcultaDisplay.Length; i++)
            {
                dgvPalavra.Columns.Add($"col{i}", "");
                dgvPalavra.Columns[i].Width = 31;
                dgvPalavra.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // colunas existem, adiciona uma linha
            dgvPalavra.Rows.Add();
            dgvPalavra.Rows[0].Height = 30;

            // Preenche as células da linha com os caracteres da palavra oculta/descoberta
            for (int i = 0; i < palavraOcultaDisplay.Length; i++)
            {
                dgvPalavra.Rows[0].Cells[i].Value = palavraOcultaDisplay[i].ToString();
            }
        }

        private void FinalizarJogo(bool ganhou)
        {
            emJogo = false; // Sai do estado de jogo

            // Desabilitar todos os botões do alfabeto
            foreach (Button btn in botoesAlfabeto)
            {
                btn.Enabled = false;
            }

            tmrForca.Enabled = false; // Para o timer

            // Reabilitar controles para novo jogo
            btnIniciar.Enabled = true;
            txtNome.Enabled = true;
            chkComDica.Enabled = true;
            lbDica.Visible = true; // Volta a mostrar a dica, mesmo se não estiver usando.

            // Se perdeu, exiba a palavra completa
            if (!ganhou)
            {
                palavraOcultaDisplay = new StringBuilder(palavraSecreta);
                AtualizarDisplayPalavraOculta();
            }
        }

        private void AtualizarImagemForca()
        {
            ComputarImagensDaForca(true);

            BonecoEstaVivo(false);
            picBoxEspiritoBoneco.Visible = false;

            if (erros > 0 && erros <= MAX_ERROS_VISUAIS)
            {
                ComputarImagens(erros); // Chamada para exibir a próxima parte do boneco
            }
            else if (erros == 0) // Se não há erros, garanta que todas as partes do boneco enforcado estão ocultas
            {
                ComputarImagensBonecoEnforcado(false); // Oculta todas as partes do boneco enforcado
            }
        }

        private void ReabilitarBotoesAlfabeto()
        {
            foreach (Button btn in botoesAlfabeto)
            {
                btn.Enabled = true;
            }
        }
        private void DesabilitarTeclado()
        {
            foreach (Button btn in botoesAlfabeto)
            {
                btn.Enabled = false;
            }
        }

        // Função para adicionar os botões do alfabeto à lista
        private void AdicionarBotoesAlfabeto(params Button[] buttons)
        {
            botoesAlfabeto.AddRange(buttons);
        }
        private void ResetarJogo()
        {
            FinalizarJogo(false); 
        }

        private void processarLetraClicada(char letra)
        {
            // Converte a letra para maiúscula para padronização na comparação
            letra = char.ToUpper(letra);

            string palavra = dicio[indiceAleatorio].Palavra.Trim(); // Obter a palavra do dicionário

            bool achouLetraNaPalavra = false;

            // Itera sobre a palavra secreta para verificar a letra
            for (int i = 0; i < palavra.Length; i++)
            {
                // Converte o caractere da palavra para maiúscula para comparação case-insensitive
                if (char.ToUpper(palavra[i]) == letra)
                {
                    pontos++; // Incrementa os pontos
                    achouLetraNaPalavra = true;

                    // Atualiza o display da palavra oculta com la letra encontrada
                    palavraOcultaDisplay[i] = letra;
                    // E atualiza a célula correspondente no DataGridView
                    if (dgvPalavra.Columns.Count > i && dgvPalavra.Rows.Count > 0)
                    {
                        dgvPalavra.Rows[0].Cells[i].Value = letra.ToString();
                    }
                }
            }

            nome = textNome.Text; // Pega o nome do jogador do campo de texto

            if (!achouLetraNaPalavra) // Se a letra não foi achada na palavra
            {
                erros++; // Erros é acrescido
                // Atualiza o label de erros
                lbErros.Text = $"Erros: {erros}";

                // Chama AtualizarImagemForca para exibir a próxima parte do boneco ou a troca de rosto
                AtualizarImagemForca();

                // Verifica se o número de erros atingiu o limite para perda do jogo
                if (erros == LIMITE_ERROS_PARA_PERDER) // Quando 'erros' chega a 9
                {
                    // Oculta todos os botões do teclado imediatamente
                    DesabilitarTeclado();

                    // Chama PerdeuOJogo que iniciará a sequência de troca de rosto (se erro 9)
                    // e o timer para a exibição do espírito.
                    PerdeuOJogo();
                }
            }
            else // Se a letra for encontrada
            {
                lbPontos.Text = $"Pontos: {pontos}"; // Atualiza o label de pontos

                // Verifica se a palavra foi completamente adivinhada
                // Compara o display atual (sem underlines ou caracteres especiais que não sejam letras)
                // com a palavra secreta original (apenas letras).
                string displayAtualLimpo = new string(palavraOcultaDisplay.ToString().Where(char.IsLetter).ToArray());
                string palavraSecretaLimpa = new string(palavraSecreta.Where(char.IsLetter).ToArray());

                if (displayAtualLimpo.Length == palavraSecretaLimpa.Length)
                {
                    // Oculta todos os botões do teclado
                    DesabilitarTeclado();
                    tmrForca.Enabled = false; // Para o timer de tempo, se estiver ativo

                    ImagensGanhou(); // Exibe as imagens de vitória
                    MessageBox.Show($"Parabéns, {nome}! \nVocê ganhou o jogo da forca!", "BOM JOGO!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    ResetarJogo(); // Reinicia ou prepara para um novo jogo
                }
            }
        }

        // =========================== BOTÕES DO FORMULARIO ===========================
        private void btnA_Click(object sender, EventArgs e)
        {
            processarLetraClicada('A');
            ((Button)sender).Enabled = false; // Desativa o botão 'A' após o clique
        }
        private void btnB_Click(object sender, EventArgs e)
        {
            processarLetraClicada('B');
            ((Button)sender).Enabled = false;
        }
        private void btnC_Click(object sender, EventArgs e)
        {
            processarLetraClicada('C');
            ((Button)sender).Enabled = false;
        }
        private void btnD_Click(object sender, EventArgs e)
        {
            processarLetraClicada('D');
            ((Button)sender).Enabled = false;
        }
        private void btnE_Click(object sender, EventArgs e)
        {
            processarLetraClicada('E');
            ((Button)sender).Enabled = false;
        }
        private void btnF_Click(object sender, EventArgs e)
        {
            processarLetraClicada('F');
            ((Button)sender).Enabled = false;
        }
        private void btnG_Click(object sender, EventArgs e)
        {
            processarLetraClicada('G');
            ((Button)sender).Enabled = false;
        }
        private void btnH_Click(object sender, EventArgs e)
        {
            processarLetraClicada('H');
            ((Button)sender).Enabled = false;
        }
        private void btnI_Click(object sender, EventArgs e)
        {
            processarLetraClicada('I');
            ((Button)sender).Enabled = false;
        }
        private void btnJ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('J');
            ((Button)sender).Enabled = false;
        }
        private void btnK_Click(object sender, EventArgs e)
        {
            processarLetraClicada('K');
            ((Button)sender).Enabled = false;
        }
        private void btnL_Click(object sender, EventArgs e)
        {
            processarLetraClicada('L');
            ((Button)sender).Enabled = false;
        }
        private void btnM_Click(object sender, EventArgs e)
        {
            processarLetraClicada('M');
            ((Button)sender).Enabled = false;
        }
        private void btnN_Click(object sender, EventArgs e)
        {
            processarLetraClicada('N');
            ((Button)sender).Enabled = false;
        }
        private void btnO_Click(object sender, EventArgs e)
        {
            processarLetraClicada('O');
            ((Button)sender).Enabled = false;
        }
        private void btnP_Click(object sender, EventArgs e)
        {
            processarLetraClicada('P');
            ((Button)sender).Enabled = false;
        }
        private void btnQ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Q');
            ((Button)sender).Enabled = false;
        }
        private void btnR_Click(object sender, EventArgs e)
        {
            processarLetraClicada('R');
            ((Button)sender).Enabled = false;
        }
        private void btnS_Click(object sender, EventArgs e)
        {
            processarLetraClicada('S');
            ((Button)sender).Enabled = false;
        }
        private void btnT_Click(object sender, EventArgs e)
        {
            processarLetraClicada('T');
            ((Button)sender).Enabled = false;
        }
        private void btnU_Click(object sender, EventArgs e)
        {
            processarLetraClicada('U');
            ((Button)sender).Enabled = false;
        }
        private void btnV_Click(object sender, EventArgs e)
        {
            processarLetraClicada('V');
            ((Button)sender).Enabled = false;
        }
        private void btnW_Click(object sender, EventArgs e)
        {
            processarLetraClicada('W');
            ((Button)sender).Enabled = false;
        }
        private void btnX_Click(object sender, EventArgs e)
        {
            processarLetraClicada('X');
            ((Button)sender).Enabled = false;
        }
        private void btnY_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Y');
            ((Button)sender).Enabled = false;
        }
        private void btnZ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Z');
            ((Button)sender).Enabled = false;
        }
        private void btnÇ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ç');
            ((Button)sender).Enabled = false;
        }
        private void btnÁ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Á');
            ((Button)sender).Enabled = false;
        }
        private void btnÂ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Â');
            ((Button)sender).Enabled = false;
        }
        private void btnÃ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ã');
            ((Button)sender).Enabled = false;
        }
        private void btnÉ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('É');
            ((Button)sender).Enabled = false;
        }
        private void btnÊ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ê');
            ((Button)sender).Enabled = false;
        }
        private void btnÍ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Í');
            ((Button)sender).Enabled = false;
        }
        private void btnÓ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ó');
            ((Button)sender).Enabled = false;
        }
        private void btnÔ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ô');
            ((Button)sender).Enabled = false;
        }
        private void btnÕ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Õ');
            ((Button)sender).Enabled = false;
        }
        private void btnÚ_Click(object sender, EventArgs e)
        {
            processarLetraClicada('Ú');
            ((Button)sender).Enabled = false;
        }
        private void btnHifen_Click(object sender, EventArgs e)
        {
            processarLetraClicada('-');
            ((Button)sender).Enabled = false;
        }
        private void btnEspaco_Click(object sender, EventArgs e)
        {
            processarLetraClicada(' ');
            ((Button)sender).Enabled = false;
        }


        private void AtualizarTela()
        {
            dicio.ExibirDados(dgvPalavrasEDicas);

            if (!dicio.EstaVazio && dicio.RegistroAtual != null)
            {
                txtRA.Text = dicio.RegistroAtual.Palavra;
                txtNome.Text = dicio.RegistroAtual.Dica;

                if (dicio.PosicaoAtual >= 0 && dicio.PosicaoAtual < dgvPalavrasEDicas.Rows.Count)
                {
                    dgvPalavrasEDicas.ClearSelection();
                    dgvPalavrasEDicas.Rows[dicio.PosicaoAtual].Selected = true;
                    dgvPalavrasEDicas.FirstDisplayedScrollingRowIndex = dicio.PosicaoAtual;
                }
            }
            else
            {
                txtRA.Text = "";
                txtNome.Text = "";
                dgvPalavrasEDicas.ClearSelection();
            }

            slRegistro.Text = $"Registro: {(dicio.EstaVazio ? 0 : dicio.PosicaoAtual + 1)}/{dicio.Tamanho}";

            // Habilita/Desabilita botões de navegação
            btnInicio.Enabled = !dicio.EstaVazio && !dicio.EstaNoInicio;
            btnAnterior.Enabled = !dicio.EstaVazio && !dicio.EstaNoInicio;
            btnProximo.Enabled = !dicio.EstaVazio && !dicio.EstaNoFim;
            btnFim.Enabled = !dicio.EstaVazio && !dicio.EstaNoFim;

            // Gerenciamento de botões de CRUD conforme SituacaoAtual
            bool podeNavegar = dicio.SituacaoAtual == VetorDicionario.Situacao.navegando;
            btnNovo.Enabled = podeNavegar;
            btnEditar.Enabled = podeNavegar && !dicio.EstaVazio;
            btnExcluir.Enabled = podeNavegar && !dicio.EstaVazio;
            btnCancelar.Enabled = dicio.SituacaoAtual != VetorDicionario.Situacao.navegando;
            btnSair.Enabled = true;
        }

        // =========================== Imagens ===========================

        public void ComputarImagens(int erro)
        {
            switch (erro)
            {
                case 1:
                    picBoxForca1.Visible = true;
                    break;
                case 2:
                    picBoxForca2.Visible = true;
                    break;
                case 3:
                    picBoxForca3.Visible = true;
                    break;
                case 4:
                    picBoxForca4.Visible = true;
                    break;
                case 5:
                    picBoxForca5.Visible = true;
                    break;
                case 6:
                    picBoxForca6.Visible = true;
                    break;
                case 7:
                    picBoxForca7.Visible = true;
                    break;
                case 8:
                    picBoxForca8.Visible = true;
                    break;
                case ERRO_TROCA_ROSTO: // Quando o erro for 9
                    picBoxForca1.Visible = false; // Oculta a primeira imagem do rosto
                    picBoxBonecoXX.Visible = true; // Exibe a imagem do rosto com XX                      
                    break;
            }
        }

        public void ResetarImagens()
        {
            // Garante que a forca base esteja visível.
            ComputarImagensDaForca(true);

            ComputarImagensBonecoEnforcado(false);

            BonecoEstaVivo(false);

            picBoxEspiritoBoneco.Visible = false;
            picBoxEspiritoBoneco.Left = 108;
            picBoxEspiritoBoneco.Top = 129;
            tmrEspiritoBoneco.Enabled = false;
        }

        public void ComputarImagensDaForca(bool estado) // muda a visibilidade da forca de acordo com a variável bool estado
        {
            picForcaUm.Visible = estado;
            picForcaDois.Visible = estado;
            picForcaTres.Visible = estado;
            picForcaQuatro.Visible = estado;
            picForcaCinco.Visible = estado;
            picForcaSeis.Visible = estado;
            picForcaSete.Visible = estado;
        }

        public void ComputarImagensBonecoEnforcado(bool estado) // muda a visibilidade da forca de acordo com a variável bool estado
        {
            picBoxForca1.Visible = estado;
            picBoxForca2.Visible = estado;
            picBoxForca3.Visible = estado;
            picBoxForca4.Visible = estado;
            picBoxForca5.Visible = estado;
            picBoxForca6.Visible = estado;
            picBoxForca7.Visible = estado;
            picBoxForca8.Visible = estado;
            picBoxBonecoXX.Visible = estado;
        }

        public void BonecoEstaVivo(bool estado) // muda a visibilidade do vivo de acordo com a variável bool estado
        {
            picBoxBandeiraUm.Visible = estado;
            picBoxBandeiraDois.Visible = estado;
            picBoxBandeiraTres.Visible = estado;
            picBoxBonecoVivo.Visible = estado;
        }

        public void ImagensGanhou() // combinação imagens de vitória
        {
            ComputarImagensDaForca(false);
            ComputarImagensBonecoEnforcado(false);
            BonecoEstaVivo(true);

            picBoxEspiritoBoneco.Visible = false;
        }

        public void PerdeuOJogo() // combinação imagens de derrota
        {

            // Garante que todas as partes do enforcado estão visíveis antes do temporizador do espírito.
            ComputarImagensBonecoEnforcado(true); // Exibe todas as partes do enforcado

            if (erros == ERRO_TROCA_ROSTO)
            {
                picBoxForca1.Visible = false;
                picBoxBonecoXX.Visible = true;
            }
            else
            {
                picBoxForca1.Visible = true;
                picBoxBonecoXX.Visible = false;
            }

            // Desabilitar o teclado imediatamente para evitar mais interações
            DesabilitarTeclado();

            
            tmrEspiritoBoneco.Interval = 1000;          // Configurar o timer para o atraso de 1 segundo
            tmrEspiritoBoneco.Tag = "ExibirEspirito";   // Usa a propriedade Tag para identificar a ação do timer
            tmrEspiritoBoneco.Enabled = true;           // Inicia o timer
        }

        private void tmrBarraDeStatus_Tick(object sender, EventArgs e)
        {
            string data = DateTime.Now.ToLongDateString(); // data atual
            string hora = DateTime.Now.ToLongTimeString(); // hora atual

            // Certifique-se de que tabControl1 não é nulo antes de tentar acessar SelectedTab
            if (tabControl1 == null)
            {
                return; // Sai se o TabControl não estiver inicializado
            }

            // Verifica qual aba está selecionada
            if (tabControl1.SelectedTab == tpForca) // Se a aba atual do tabControl1 for a Forca
            {
                slRegistro.Text = $"    Data: {data}  Hora: {hora}    Inspirado em: {inspiracao}";
            }
        }

        private void btnJogarNovamente_Click(object sender, EventArgs e)
        {
            // Limpar o campo de texto do nome
            textNome.Clear();
            textNome.Enabled = true; // Reabilita para uma nova entrada de nome

            // Reabilitar todas as letras do formulário (botões do teclado)
            ReabilitarBotoesAlfabeto(); 

            // Limpar o dgvPalavra (o DataGridView que exibe a palavra oculta)
            dgvPalavra.Rows.Clear();
            dgvPalavra.Columns.Clear(); 
                                        
            // Limpar os Labels (lbDica, lbPontos, lbErros, lbTempo)
            lbDica.Text = "Dica:";
            lbDica.Visible = false;

            lbPontos.Text = "Pontos: 0";
            lbErros.Text = "Erros: 0";
            lbTempo.Text = "Tempo Restante: --s";
            lbTempo.Visible = false;

            // Resetar as variáveis de controle do jogo
            pontos = 0;
            erros = 0;
            emJogo = false; // O jogo não está mais em andamento
            tempoForca = 0; // Zera o tempo

            tmrForca.Enabled = false;

            // Resetar as imagens da forca e do boneco
            ResetarImagens();

            btnIniciar.Enabled = true; // Habilita o botão Iniciar para uma nova partida
            btnJogarNovamente.Enabled = false; // Desabilita o botão Jogar Novamente

            chkComDica.Enabled = true;
            chkComDica.Checked = false;
        }

        private void tmrEspiritoBoneco_Tick(object sender, EventArgs e)
        {
            // Verifica se o timer está sendo usado para o atraso antes do espírito aparecer
            if (tmrEspiritoBoneco.Tag != null && tmrEspiritoBoneco.Tag.ToString() == "ExibirEspirito")
            {
                tmrEspiritoBoneco.Enabled = false;
                tmrEspiritoBoneco.Tag = null;

                ComputarImagensBonecoEnforcado(false); // Oculta todas as partes do boneco enforcado
                picBoxForca1.Visible = false;
                picBoxBonecoXX.Visible = false;

                picBoxEspiritoBoneco.Visible = true; // Exibe a imagem do espírito
                tmrEspiritoBoneco.Interval = 50; // Restaura o intervalo para o movimento do espírito
                tmrEspiritoBoneco.Enabled = true; // Reinicia o timer para o movimento do espírito

                // reinicia o jogo ou prepara para um novo
                MessageBox.Show($"Ah não, {nome}! \nVocê não ganhou o jogo da forca. \nA palavra correta era: {palavraSecreta}", "NÃO FOI DESTA VEZ, TENTE NOVAMENTE!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                ResetarJogo(); // Reinicia ou prepara para um novo jogo
            }
            else  // Movimento do espírito
            {
                picBoxEspiritoBoneco.Top -= 5;
                if (picBoxEspiritoBoneco.Top < 0)
                {
                     picBoxEspiritoBoneco.Top = 129;
                }
            }
        }

        private void tmrForca_Tick(object sender, EventArgs e)
        {
            nome = textNome.Text;

            if (emJogo && chkComDica.Checked)
            {
                tempoForca--;
                lbTempo.Text = $"Tempo Restante: {tempoForca}s";

                if (tempoForca <= 0)
                {
                    tmrForca.Enabled = false;

                    MessageBox.Show($"Ah não {nome}!, o tempo se esgotou! \nVocê não ganhou o jogo da forca.", "FIM DE JOGO - TEMPO ESGOTADO", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FinalizarJogo(false); // Termina o jogo como perda
                }
            }
        }
    }

}
