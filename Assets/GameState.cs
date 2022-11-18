using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    
    public GameState()
    {
        worlds = new();
        bank = new();
    }

    public List<world> worlds;
    public Dictionary<Pyramid, int> bank;
    public override string ToString()
    {
        List<string> lines = new();

        foreach(world world in worlds)
        {
            lines.Add(world.ToString());
        }
        lines.Add("The bank contains:");
        foreach(Pyramid pyramid in bank.Keys)
        {
            if (bank[pyramid] > 0)
            {
                lines.Add($"{bank[pyramid]} pyramids of " + pyramid.ToString());
            }
        }

        return string.Join("\n", lines);
    }

    public void InitStash(int players = 2)
    {
        bank = new Dictionary<Pyramid, int>();
        bank[new(1, Color.Red)] = players + 1;
        bank[new(2, Color.Red)] = players + 1;
        bank[new(3, Color.Red)] = players + 1;
        bank[new(1, Color.Blue)] = players + 1;
        bank[new(2, Color.Blue)] = players + 1;
        bank[new(3, Color.Blue)] = players + 1;
        bank[new(1, Color.Green)] = players + 1;
        bank[new(2, Color.Green)] = players + 1;
        bank[new(3, Color.Green)] = players + 1;
        bank[new(1, Color.Yellow)] = players + 1;
        bank[new(2, Color.Yellow)] = players + 1;
        bank[new(3, Color.Yellow)] = players + 1;
    }
    
    public bool TrySpendFromStash(Pyramid pyramid)
    {
        var py = new Pyramid(pyramid);
        try { 
        if (bank[pyramid]>0)
        {
            bank[pyramid]--;
            return true;
        } return false;
        } catch (System.Exception e) {
            foreach(var key in bank.Keys)
                Debug.Log(key);
            throw e;
        }
    }

    public void AddToStash(IEnumerable<Pyramid> pyramids)
    {
        foreach(var py in pyramids)
        {
            AddToStash(py);
        }
    }

    public void AddToStash(Pyramid pyramid)
    {
        bank[pyramid]++;
    }
}

public class History
{
    GameState state;
    int index;
    List<TurnAction> moves;

    public History(int numPlayers)
    {
        state = new GameState();
        state.InitStash(numPlayers);
        index = 0;
        moves = new List<TurnAction>();
    }

    public bool LogMove(TurnAction move, bool revisionist = false)
    {
        if (index == moves.Count)
        {
            if (move.Execute(state))
            {
                moves.Add(move);
                return true;
            }
            return false;
        }
        if(moves[index] == move || revisionist)
        {
            moves[index] = move;
            return move.Execute(state);
        }
        return false;
    } 

    public void Rewind()
    {
        index--;
        moves[index].Rewind(state);
    }
    public void Redo()
    {
        moves[index].Execute(state);
        index++;
    }

    public override string ToString()
    {
        return state.ToString();
        //also all the moves
    }
}

public enum Color
{
    Red,
    Green,
    Blue,
    Yellow
}
public class Pyramid
{
    public int size;
    public Color color;
    public override string ToString()
    {
        return $"size {size} colored {color}";
    }

    public Pyramid() { }//allow block constructors
    public Pyramid(int size, Color color)
    {
        this.size = size;
        this.color = color;
    }

    public Pyramid(Pyramid toClone)
    {
        this.size = toClone.size;
        this.color = toClone.color;
    }

    public override bool Equals(object obj)
    {
        Pyramid pyramid = obj as Pyramid;
        return (pyramid.color == color && pyramid.size == size);
    }

    public override int GetHashCode()
    {
        return size.GetHashCode() * 17 + color.GetHashCode();
    }
}
public class Ship : Pyramid
{
    public int owner;
    public Ship(Pyramid source, int owner)
    {
        color = source.color;
        size = source.size;
    }
    public override string ToString()
    {
        return $"A {color} ship belonging to player {owner} of size {size}";
    }

}
public class Star : Pyramid
{
    public Star(Pyramid source)
    {
        this.color = source.color;
        this.size = source.size;
    }

    public override string ToString()
    {
        return $"A {color} star of size {size}";
    }
}

public class world
{
    public int playerHW = -1;
    public List<Star> stars;
    public List<Ship> ships;

    public override string ToString()
    {
        List<string> lines = new();

        if (playerHW != -1) lines.Add($"Player {playerHW}'s homeworld. It contains: ");
        foreach (Star star in stars)
        {
            lines.Add(star.ToString());
        }

        return string.Join("\n", lines);
    }
}