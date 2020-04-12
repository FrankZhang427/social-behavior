using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Planner {

    private Type methodsType;
    private Type operatorsType;
    private int searchDepth = 30;
    private bool cancelSearch;
    private List<string> primitive = new List<string>();
    private Dictionary<string, List<string>> compound = new Dictionary<string, List<string>>();
    public bool CancelSearch { set { cancelSearch = value;}}

    public Planner(Type methodsType, Dictionary<string, MethodInfo[]> methodsDict, Type operatorsType)
    {
        this.methodsType = methodsType;
        this.operatorsType = operatorsType;
        InitializePlanner(methodsDict);
    }

    private void InitializePlanner(Dictionary<string, MethodInfo[]> methodsDict)
    {
        DeclareOperators();
        foreach (KeyValuePair<string, MethodInfo[]> method in methodsDict)
        {
            DeclareMethods(method.Key, method.Value);
        }
    }

    public List<string> DeclareOperators()
    {
        MethodInfo[] methodInfos = operatorsType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);

        primitive = new List<string>();

        foreach (MethodInfo info in methodInfos)
        {
            if (info.ReturnType.Name.Equals("State"))
            {
                string methodName = info.Name;
                if (!primitive.Contains(methodName))
                    primitive.Add(methodName);
            }
        }

        return primitive;
    }

    public List<string> DeclareMethods(string taskName, MethodInfo[] methodInfos)
    {
        List<string> methodList = new List<string>();

        foreach (MethodInfo info in methodInfos)
        {
            if (info != null && info.ReturnType.Name.Equals("List`1"))
            {
                methodList.Add(info.Name);
            }
        }

        if (compound.ContainsKey(taskName))
            compound.Remove(taskName);
        compound.Add(taskName, methodList);

        return compound[taskName];
    }


    public List<string> SolvePlanningProblem(State state, List<List<string>> tasks)
    {
        List<string> result = SeekPlan(state, tasks, new List<string>(), 0);
        if (cancelSearch) cancelSearch = false;
        return result;
    }


    public List<string> SeekPlan(State state, List<List<string>> tasks, List<string> plan, int depth)
    {
        if (cancelSearch) return null;

        // safety
        if (searchDepth > 0)
        {
            if (depth >= searchDepth)
                return null;
        }

        if (tasks.Count == 0) return plan;

        List<string> task = tasks[0];
        if (primitive.Contains(task[0]))
        {

            MethodInfo info = operatorsType.GetMethod(task[0]);
            object[] parameters = new object[task.Count];
            parameters[0] = new State(state);
            if (task.Count > 1)
            {
                int x = 1;
                List<string> paramets = task.GetRange(1, (task.Count - 1));
                foreach (string param in paramets)
                {
                    parameters[x] = param;
                    x++;
                }
            }
            State newState = (State)info.Invoke(null, parameters);

            if (newState != null)
            {
                string toAddToPlan = "(" + task[0];
                if (task.Count > 1)
                {
                    List<string> paramets = task.GetRange(1, (task.Count - 1));
                    foreach (string param in paramets)
                    {
                        toAddToPlan += (", " + param);
                    }
                }
                toAddToPlan += ")";
                plan.Add(toAddToPlan);
                List<string> solution = SeekPlan(newState, tasks.GetRange(1, (tasks.Count - 1)), plan, (depth + 1));
                if (solution != null)
                    return solution;
            }
        }
        if (compound.ContainsKey(task[0]))
        {
            List<string> relevant = compound[task[0]];
            foreach (string method in relevant)
            {
                // Decompose non-primitive task into subtasks by use of a HTN method 
                MethodInfo info = methodsType.GetMethod(method);
                object[] parameters = new object[task.Count];
                parameters[0] = new State(state);
                if (task.Count > 1)
                {
                    int x = 1;
                    List<string> paramets = task.GetRange(1, (task.Count - 1));
                    foreach (string param in paramets)
                    {
                        parameters[x] = param;
                        x++;
                    }
                }
                List<List<string>> subtasks = null;
                try
                {
                    subtasks = (List<List<string>>)info.Invoke(null, parameters);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }

                if (subtasks != null)
                {
                    List<List<string>> newTasks = new List<List<string>>(subtasks);
                    newTasks.AddRange(tasks.GetRange(1, (tasks.Count - 1)));
                    try
                    {
                        List<string> solution = SeekPlan(state, newTasks, plan, (depth + 1));
                        if (solution != null)
                            return solution;
                    }
                    catch (StackOverflowException e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
        return null;
    }
}
