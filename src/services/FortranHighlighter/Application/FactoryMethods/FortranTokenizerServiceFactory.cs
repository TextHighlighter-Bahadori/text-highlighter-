namespace FortranHighlighter.Application.FactoryMethods;

public class FortranTokenizerServiceFactory : IFortranTokenizerServiceFactory
{
    public IFortranTokenizerService Create(string code) => new FortranTokenizerService(code);
}