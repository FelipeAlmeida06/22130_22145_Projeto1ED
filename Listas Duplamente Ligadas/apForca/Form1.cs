// Nome: Felipe Antônio de Oliveira Almeida     RA: 22130
// Nome: Miguel de Castro Chaga Silva           RA: 22145

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static System.Resources.ResXFileRef;

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
    }

}
