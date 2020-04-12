using System;
using System.Collections.Generic;
using System.Reflection;

public class Domain {

    private static void AddTask(List<List<string>> result, params string[] values)
    {
        try { result.Add(new List<string>(values));}
        catch (StackOverflowException) { }
    }

    // compound tasks
    public static List<List<string>> MoveTo_m(State state, string alcove)
    {
        List<List<string>> result = new List<List<string>>();

        if (state.ContainsVar("at") && !state.CheckVar("at", alcove))
        {
            string location = state.GetStateOfVar("at")[0];
            List<string> closestAlcove = state.GetStateOfRelation("closest", location);
            bool foundPath = false;
            foreach (string nextAlcove in closestAlcove)
            {
                if (state.CheckRelation("closest", nextAlcove, alcove))
                {
                    foundPath = true;
                    AddTask(result, "MoveTo", nextAlcove);
                    break;
                }
            }
            if (!foundPath)
            {
                foreach (string nextAlcove in closestAlcove)
                {
                    List<string> moreAlcove = state.GetStateOfRelation("closest", nextAlcove);
                    foreach (string otherAlcove in moreAlcove)
                    {
                        if (otherAlcove != location && state.CheckRelation("closest", otherAlcove, alcove))
                        {
                            foundPath = true;
                            AddTask(result, "MoveTo", nextAlcove);
                            break;
                        }
                    }
                }
            }
            if (!foundPath)
            {
                bool allAlcoveChecked = true;
                foreach (string _alcove in closestAlcove)
                {
                    if (!state.CheckVar("checked", _alcove))
                    {
                        allAlcoveChecked = false;
                        AddTask(result, "MoveTo", _alcove);
                        break;
                    }
                }
                if (allAlcoveChecked) return null;
            }
        }
        else return null;
        return result;
    }


    public static List<List<string>> Hide_m(State state)
    {
        List<List<string>> returnVal = new List<List<string>>();

        if (state.ContainsVar("hide"))
        {
            string alcove = state.GetStateOfVar("at")[0];
            if (state.CheckVar("hide", alcove))
            {
                AddTask(returnVal, "HideInAlcove", alcove);
                AddTask(returnVal, "hide");
            }
            else
            {
                string notHideAlcove = state.GetStateOfVar("hide")[0];
                AddTask(returnVal, "MoveTo", notHideAlcove);
                AddTask(returnVal, "hide");
            }
        }
        else
        {
            AddTask(returnVal, "Finish");
        }

        return returnVal;
    }

    // Primitive tasks
    public static State MoveTo(State state, string alcove)
    {
        State newState = state;
        if (state.ContainsVar("at") && !state.CheckVar("at", alcove))
        {
            string location = state.GetStateOfVar("at")[0];
            // check condition
            if (state.CheckRelation("closest", location, alcove))
            {
                // add effects
                newState.Remove("at", location);
                newState.Add("at", alcove);
                newState.Add("checked", alcove);
            }
            else return null;
        }
        return newState;
    }

    public static State Hide(State state, string alcove)
    {
        State newState = state;
        if (state.CheckVar("at", alcove) && state.CheckVar("hide", alcove))
        {
            newState.Remove("hide", alcove);
            newState.Add("notHide", alcove);
            List<string> checkedList = new List<string>(newState.GetStateOfVar("checked"));
            foreach (string checkedAlcove in checkedList)
            {
                if (checkedAlcove != alcove)
                    newState.Remove("checked", checkedAlcove);
            }
        }
        return newState;
    }

    public static State Finish(State state)
    {
        State newState = state;
        newState.Add("finished", "true");
        return newState;
    }

    public Dictionary<string, MethodInfo[]> GetMethodsDict()
    {
        Dictionary<string, MethodInfo[]> dict = new Dictionary<string, MethodInfo[]>();
        MethodInfo[] moveInfos = new MethodInfo[] { this.GetType().GetMethod("MoveTo_m") };
        dict.Add("MoveTo", moveInfos);
        MethodInfo[] hideInfos = new MethodInfo[] { this.GetType().GetMethod("Hide_m") };
        dict.Add("Hide", hideInfos);
        return dict;
    }
}
