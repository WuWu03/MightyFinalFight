namespace GameFrameWork.Localization
{
    public interface ILanguageLoader
    {
        public string GetLanguageText(string key);

        public string GetLanguageText(int id);
    }
}