namespace FortranHighlighter.Application.FactoryMethods;

public interface IFortranTokenizerServiceFactory
{
    public IFortranTokenizerService Create(string code);
}