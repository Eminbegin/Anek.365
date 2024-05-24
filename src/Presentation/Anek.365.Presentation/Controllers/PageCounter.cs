namespace Anek._365.Presentation.Controllers;

public static class PageCounter
{
    public static int MaxPage(int pageSize, int elementsCount)
        => ((elementsCount - 1) / pageSize) + 1;
}