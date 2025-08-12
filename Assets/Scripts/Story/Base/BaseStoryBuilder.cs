using System.Collections.Generic;

public abstract class BaseStoryBuilder : IStoryBuilder
{
    public abstract void BuildStory(Queue<BaseStory> stories);
}
