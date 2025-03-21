using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;

namespace SspnetSDK.Editor.BuildUtils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class FixProblemInstruction
    {
        private readonly string desc;
        private readonly bool isAutoresolvePossible;
        private bool _checkedForResolve;

        public FixProblemInstruction(string description, bool autoresolve)
        {
            desc = description;
            isAutoresolvePossible = autoresolve;
        }

        public bool checkedForResolve
        {
            get => _checkedForResolve;
            set
            {
                if (isAutoresolvePossible) _checkedForResolve = value;
            }
        }

        public string getDescription()
        {
            return desc;
        }

        public bool canBeResolvedAutomatically()
        {
            return isAutoresolvePossible;
        }

        public virtual void fixProblem()
        {
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public abstract class CheckingStep
    {
        public bool done;
        public abstract string getName();
        public abstract List<FixProblemInstruction> check();
        public abstract bool isRequiredForPlatform(BuildTarget target);
    }
}