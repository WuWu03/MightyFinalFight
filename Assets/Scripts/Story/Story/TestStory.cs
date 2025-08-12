using System.Collections.Generic;
using UnityEngine;

public class TestStory : IStoryBuilder
{
    public void BuildStory(Queue<BaseStory> stories)
    {
        //storyQueue.Enqueue("Welcome to the world of adventure!");
        //storyQueue.Enqueue("Your journey begins here.");
        //storyQueue.Enqueue("Explore the surroundings and find hidden treasures.");
        //storyQueue.Enqueue("Beware of the dangers that lurk in the shadows.");
        //storyQueue.Enqueue("Good luck, brave adventurer!");
    }

    public void EntityMove(int entityId, Vector2 pos)
    {


    }

    private List<BaseStory> m_Stories = null;
}
