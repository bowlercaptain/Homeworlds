using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TurnAction
{
    //types:
    /* 
     
    Moves from SDG:

    homeworld star1 star2 ship -- done
    discover ship fromSystem newStar newName
    move ship fromSystem toSystem
    build ship inSystem
    trade oldShip newShip inSystem
    attack ship inSystem
    sacrifice ship inSystem
    catastrophe inSystem color
    pass

    Kaylo Naivaklam's alt-color proposals:
    Orange: Hyperspace navigation - Move between systems with the same(*1) star size.
Cyan: Stellar transmutation - change the color of a star. Like blue, but for stars instead of ships.
Black: Fusion - Swell the size of a star to the next size of that color available in the stash. Not applicable to large stars.
White: Fission - Split a star. A large star becomes a medium and a small (*2) of the same color(*3), a medium star becomes two smalls of the same color, a small star is returned to the bank (poof).
Opaque Green: Kickstart - Allows "kicked" sacrifice of another ship, which produces one more action than a regular sacrifice. (*4)
Pink: Gnaw - Reduce the size of an opponent's ship to the next smaller available size of that color, or destroy it if there are no smaller sizes available.
Clear: Mercenary - When grown via green, acts as if it were grown twice. All mercenariy ships belong to the current player (they change owners every turn). No intrinsic power.
Chrome: Reflect - May make use of opponent colors. (*5)
Purple: Broadcast - Allows access to colors provided by your ships in other systems where you have access to purple. (*6)
Grey: Stevedore - Allows nesting and un-nesting of your ships (*7)
Opaque Red: - TBA, pondering some sort of "Martian Colonist", allowing you to relocate your homeworld.

    personal proposals:

    Fused-color ships (stevedoria?)
    plus-sized ships and stars

     */

    public abstract bool Execute(GameState state);
    

public abstract bool Rewind(GameState state);
}

public class pass : TurnAction
{
    public override bool Execute(GameState state)
    {
        return true;   
    }

    public override bool Rewind(GameState state)
    {
        return true;
    }
}

public class Homeworld : TurnAction
{
    world world;
    List<Pyramid> spentPyramids;


    public Homeworld(Pyramid planet1, Pyramid planet2, Color shipColor, int playerId)
    {
        spentPyramids = new();
        world = new world { playerHW = playerId, ships = new List<Ship> { new Ship ( new Pyramid{ color = shipColor, size = 3 }, owner: playerId  ) }, stars = new List<Star> { new Star(planet1), new Star(planet2) } };
        spentPyramids.Add(new Pyramid { color = shipColor, size = 3 });
        spentPyramids.Add(planet1);
        spentPyramids.Add(planet2);
    }

    public override bool Execute(GameState state)
    {   
        foreach(Pyramid pyramid in spentPyramids)
        {
            if (!state.TrySpendFromStash(pyramid)) return false;
        }
        state.worlds.Add(world);
        return true;
    }

    public override bool Rewind(GameState state)
    {
        foreach(Pyramid pyramid in spentPyramids)
        {
            state.AddToStash(pyramid);
        }
        return state.worlds.Remove(world);
    }
}

public class Move : TurnAction
{
    public world departure;
    public world arrival;
    public Ship moving;

    public override bool Execute(GameState state)
    {
        
        if (departure.ships.Remove(moving))
        {
            arrival.ships.Add(moving);
            if (!state.worlds.Contains(arrival))
            {
                state.worlds.Add(arrival);
            }
            if(departure.ships.Count == 0)
            {
                state.AddToStash(departure.stars);
                state.worlds.Remove(departure);
            }
            return true;
        } return false;
    }

    public override bool Rewind(GameState state)
    {
        if (arrival.ships.Remove(moving))
        {
            departure.ships.Add(moving);
            if (!state.worlds.Contains(departure))
            {
                state.worlds.Add(departure);
            }
            if (arrival.ships.Count == 0)
            {
                state.AddToStash(arrival.stars);
                state.worlds.Remove(arrival);
            }
            return true;
        }
        return false;
    }
}