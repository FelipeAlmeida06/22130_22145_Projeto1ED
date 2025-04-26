using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Semelhante a classe Aluno
// Verificar com o professor se é necessário criar essa classe PalavraDica
// ou deve se usar a classe Aluno, ajustando alguns valores
public class PalavraDica : IComparable<PalavraDica>, IRegistro 
{
    const int tamanhoPalavra = 30;
   

    private string palavra;
    private string dica;

    public string Palavra { get; private set; }
    public string Dica { get; private set; }


    /*
    public string Palavra
    {
        get => palavra;
        set
        {
            if (value != "")
                palavra = value.PadRight(tamanhoPalavra, ' ').Substring(0, tamanhoPalavra);
            else
                throw new Exception("Palavra vazia é inválida.");
        }
    }

    public string Dica
    {
        get => dica;
        set
        {
            if (value != "")
                dica = value;
            else
                throw new Exception("Dica vazia é inválida.");
        }
    }
    */

    public PalavraDica(string palavra, string dica)
    {
        Palavra = palavra;
        Dica = dica;
    }

    public int CompareTo(PalavraDica outra)
    {
        return string.Compare(this.Palavra, outra?.Palavra, StringComparison.Ordinal);
    }

    public override string ToString()
    {
        return $"{palavra.Trim()} - {dica}";
    }

    public string FormatoDeArquivo()
    {
        //return $"{palavra}\n{dica}";

        return $"{Palavra}\n{Dica}";
    }
}

