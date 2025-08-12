interface IStory
{
    bool isWaitComplete { get; }
    void PlayStory();
    void PauseStory();
    void ResumeStory();
    bool IsStoryComplete();
}
