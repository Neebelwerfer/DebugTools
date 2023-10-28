using System;
using System.Reflection;
using UnityEngine;

namespace Monitoring.Display
{
    public abstract class BaseParameter
    {
        public ParameterInfo Parameter;
        public object Value;

        public abstract void OnDraw();
    }

    class IntParameter : BaseParameter
    {
        public IntParameter() => Value = 0;

        public override void OnDraw()
        {
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            Value = int.Parse(GUILayout.TextField(Value.ToString()));
        }
    }

    class FloatParameter : BaseParameter
    {
        public FloatParameter() => Value = 0f;
        public override void OnDraw()
        {
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            if (float.TryParse(GUILayout.TextField(Value.ToString()), out var res))
                Value = res;

        }
    }

    class BoolParameter : BaseParameter
    {
        public BoolParameter() => Value = false;
        public override void OnDraw()
        {
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            Value = GUILayout.Toggle((bool)Value, "");
        }
    }

    class StringParameter : BaseParameter
    {
        public StringParameter() => Value = "";
        public override void OnDraw()
        {
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            Value = GUILayout.TextField((string)Value);
        }
    }
    class Vector2Parameter : BaseParameter
    {
        public Vector2Parameter() => Value = Vector2.zero;
        public override void OnDraw()
        {
            Vector2 v = (Vector2)Value;
            float x = v.x;
            float y = v.y;
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            x = float.Parse(GUILayout.TextField(x.ToString(), 10));
            GUILayout.Label("X");
            y = float.Parse(GUILayout.TextField(y.ToString(), 10));
            GUILayout.Label("Y");
            Value = new Vector2(x, y);
        }
    }

    class Vector3Parameter : BaseParameter
    {
        public Vector3Parameter() => Value = Vector3.zero;
        public override void OnDraw()
        {
            Vector3 v = (Vector3)Value;
            float x = v.x;
            float y = v.y;
            float z = v.z;
            GUILayout.Label(Parameter.Name + ":" + Parameter.ParameterType.Name);
            x = float.Parse(GUILayout.TextField(x.ToString(), 10));
            GUILayout.Label("X");
            y = float.Parse(GUILayout.TextField(y.ToString(), 10));
            GUILayout.Label("Y");
            z = float.Parse(GUILayout.TextField(z.ToString(), 10));
            GUILayout.Label("Z");
            Value = new Vector3(x, y, z);
        }
    }

    public static class ParameterFactory
    {
        public static BaseParameter[] ProcessParameters(ParameterInfo[] parameterInfos)
        {
            if (parameterInfos.Length == 0) return null;
            BaseParameter[] parameters = new BaseParameter[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType == typeof(int))
                {
                    var param = new IntParameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                } 
                else if (parameterInfos[i].ParameterType == typeof(float))
                {
                    var param = new FloatParameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                }
                else if (parameterInfos[i].ParameterType == typeof(bool))
                {
                    var param = new BoolParameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                }
                else if (parameterInfos[i].ParameterType == typeof(string))
                {
                    var param = new StringParameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                }
                else if (parameterInfos[i].ParameterType == typeof(Vector2))
                {
                    var param = new Vector2Parameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                }
                else if (parameterInfos[i].ParameterType == typeof(Vector3))
                {
                    var param = new Vector3Parameter();
                    param.Parameter = parameterInfos[i];
                    parameters[i] = param;
                }
            }

            return parameters;
        }
    }
}
