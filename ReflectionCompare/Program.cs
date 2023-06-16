using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmitReflection;
using PtrReflection;
using System.Reflection;
using System.Diagnostics;
using ExpressionReflection;


namespace ReflectionCompare
{
    internal class Program
    {

        static void SwitchSetData(MyClass myClass, object data, string name) 
        {
            switch (name)
            {
                case "one":
                    myClass.one = (int)data;
                    break;
                case "str":
                    myClass.str = (string)data;
                    break;
                case "point":
                    myClass.point = (Vector3)data;
                    break;
                case "One":
                    myClass.One = (int)data;
                    break;
                case "Str":
                    myClass.Str = (string)data;
                    break;
                case "Point":
                    myClass.Point = (Vector3)data;
                    break;
                case "oness":
                    myClass.oness = (int[,,])data;
                    break;
                case "strss":
                    myClass.strss = (string[,])data;
                    break;
                case "pointss":
                    myClass.pointss = (Vector3[,])data;
                    break;
                case "ones":
                    myClass.ones = (int[])data;
                    break;
                case "strs":
                    myClass.strs = (string[])data;
                    break;
                case "points":
                    myClass.points = (Vector3[])data;
                    break;
            }
        }

        static object SwitchGetData(MyClass myClass, string name)
        {
            switch (name)
            {
                case "one":
                    return myClass.one;
                case "str":
                    return myClass.str;
                case "point":
                    return myClass.point;
                case "One":
                    return myClass.One;
                case "Str":
                    return myClass.Str;
                case "Point":
                    return myClass.Point;
                case "oness":
                    return myClass.oness;
                case "strss":
                    return myClass.strss;
                case "pointss":
                    return myClass.pointss;
                case "ones":
                    return myClass.ones;
                case "strs":
                    return myClass.strs;
                case "points":
                    return myClass.points;
                default:
                    return null;
            }
        }

        static unsafe void Main(string[] args)
        {
            Type type = typeof(MyClass);
            MyClass obj = new MyClass();

            Stopwatch oTime = new Stopwatch();
            int testCount = 1000000;
            int v1;
            string v2;
            Vector3 v3 = new Vector3();


            TypeAddrReflectionWrapper reflectionWrapper = new TypeAddrReflectionWrapper(typeof(MyClass));

            EmitWrapperType emitWrapperType = new EmitWrapperType(typeof(MyClass));

            ExpressionWrapperType expressionWrapperType = new ExpressionWrapperType(typeof(MyClass));


            //原生
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                obj.one = 13;
                v1 = obj.one;

                obj.str = "sad2";
                v2 = obj.str;

                obj.point = new Vector3(1.1f, 2.2f, 3.3f);
                v3 = obj.point;
            }
            oTime.Stop();
            Console.WriteLine("C#,                  字段：                                    " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                SwitchSetData(obj, 13, name);
                v1 = (int)SwitchGetData(obj, name);

                name = nameof(obj.str);
                SwitchSetData(obj, "sad2", name);
                v2 = (string)SwitchGetData(obj, name);

                name = nameof(obj.point);
                SwitchSetData(obj, new Vector3(1.1f, 2.2f, 3.3f), name);
                v3 = (Vector3)SwitchGetData(obj, name);
            }
            oTime.Stop();
            Console.WriteLine("switch,              字段：                                    " + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            Console.WriteLine();


            //指针反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                ObjReference objReference = new ObjReference(obj);
                void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
                byte** handleByte = (byte**)handleVoid;

                string name = nameof(obj.one);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetFieldValue(handleVoid, 13);
                    v1 = (int)typeAddr.ClassGetFieldValue(handleVoid);
                }

                name = nameof(obj.str);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetFieldValue(handleVoid, "sad2");
                    v2 = (string)typeAddr.ClassGetFieldValue(handleVoid);
                }

                name = nameof(obj.point);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetFieldValue(handleVoid, new Vector3(1.1f, 2.2f, 3.3f));
                    v3 = (Vector3)typeAddr.ClassGetFieldValue(handleVoid);
                }
            }
            oTime.Stop();
            Console.WriteLine("PtrReflection,       字段, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                var valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, 13);
                v1 = (int)valueWrap.getValue(obj);

                name = nameof(obj.str);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, "sad2");
                v2 = (string)valueWrap.getValue(obj);

                name = nameof(obj.point);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = (Vector3)valueWrap.getValue(obj);
            }

            oTime.Stop();
            Console.WriteLine("Emit,                字段, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            //Expression
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                var valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, 101);
                v1 = (int)valueWrap.getValue(obj);

                name = nameof(obj.str);
                valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, "abcd");
                v2 = (string)valueWrap.getValue(obj);

                name = nameof(obj.point);
                valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, new Vector3(14.5f, 1995.8f, 7.92f));
                v3 = (Vector3)valueWrap.getValue(obj);
            }
            oTime.Stop();
            Console.WriteLine("Expression,          字段, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            //原生反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                FieldInfo fieldInfo = typeof(MyClass).GetField(name);
                fieldInfo.SetValue(obj, 13);
                v1 = (int)fieldInfo.GetValue(obj);

                name = nameof(obj.str); 
                fieldInfo = typeof(MyClass).GetField(name);
                fieldInfo.SetValue(obj, "sad2");
                v2 = (string)fieldInfo.GetValue(obj);

                name = nameof(obj.point);
                fieldInfo = typeof(MyClass).GetField(name);
                fieldInfo.SetValue(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = (Vector3)fieldInfo.GetValue(obj);
            }
            oTime.Stop();
            Console.WriteLine("System.Reflection,   字段, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            Console.WriteLine();


            //指针反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                ObjReference objReference = new ObjReference(obj);
                void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
                byte** handleByte = (byte**)handleVoid;

                string name = nameof(obj.One);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetPropertyValue(handleVoid, 13);
                    v1 = (int)typeAddr.ClassGetPropertyValue(handleVoid);
                }

                name = nameof(obj.Str);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetPropertyValue(handleVoid, "sad2");
                    v2 = (string)typeAddr.ClassGetPropertyValue(handleVoid);
                }

                name = nameof(obj.Point);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.ClassSetPropertyValue(handleVoid, new Vector3(1.1f, 2.2f, 3.3f));
                    v3 = (Vector3)typeAddr.ClassGetPropertyValue(handleVoid);
                }
            }
            oTime.Stop();
            Console.WriteLine("PtrReflection,       属性, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.One);
                EmitWrapperTypeFieldAndProperty valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, 13);
                v1 = (int)valueWrap.getValue(obj);

                name = nameof(obj.Str);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, "sad2");
                v2 = (string)valueWrap.getValue(obj);

                name = nameof(obj.Point);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                valueWrap.setValue(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = (Vector3)valueWrap.getValue(obj);
            }

            oTime.Stop();
            Console.WriteLine("Emit,                属性, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            
            //Expression
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.One);
                var valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, 113);
                v1 = (int)valueWrap.getValue(obj);

                name = nameof(obj.Str);
                valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, "aad2");
                v2 = (string)valueWrap.getValue(obj);

                name = nameof(obj.Point);
                valueWrap = expressionWrapperType.GetWarp(name);
                valueWrap.setValue(obj, new Vector3(14.5f, 1995.8f, 7.92f));
                v3 = (Vector3)valueWrap.getValue(obj);
            }
            oTime.Stop();
            Console.WriteLine("Expression,          属性, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            //原生反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.One);
                PropertyInfo propertyInfo = typeof(MyClass).GetProperty(name);
                propertyInfo.SetValue(obj, 13);
                v1 = (int)propertyInfo.GetValue(obj);

                name = nameof(obj.Str);
                propertyInfo = typeof(MyClass).GetProperty(name);
                propertyInfo.SetValue(obj, "sad2");
                v2 = (string)propertyInfo.GetValue(obj);

                name = nameof(obj.Point);
                propertyInfo = typeof(MyClass).GetProperty(name);
                propertyInfo.SetValue(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = (Vector3)propertyInfo.GetValue(obj);
            }
            oTime.Stop();
            Console.WriteLine("System.Reflection,   属性, object类型：                        " + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            Console.WriteLine();


            //指针反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                ObjReference objReference = new ObjReference(obj);
                void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
                byte** handleByte = (byte**)handleVoid;

                string name = nameof(obj.one);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    *(int*)(*handleByte + typeAddr.offset) = 14;
                    v1 = *(int*)(*handleByte + typeAddr.offset);
                }

                name = nameof(obj.str);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    GeneralTool.SetObject(*handleByte + typeAddr.offset, "sa32");
                    v2 = (string)GeneralTool.VoidPtrToObject(*(void**)(*handleByte + typeAddr.offset));
                }

                name = nameof(obj.point);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    //typeAddr.ClassWriteStruct<Vector3>(handleVoid, new Vector3(1.1f, 2.2f, 3.3f));//赋值
                    //v3 = typeAddr.ClassReadStruct<Vector3>(handleVoid);//取值
                    var setData = new Vector3(1.1f, 2.2f, 3.3f);
                    GeneralTool.MemCpy(*handleByte + typeAddr.offset, GeneralTool.AsPointer(ref setData), typeAddr.stackSize);//赋值
                    GeneralTool.MemCpy(GeneralTool.AsPointer<Vector3>(ref v3), *handleByte + typeAddr.offset, typeAddr.stackSize);//取值
                }
            }
            oTime.Stop();
            Console.WriteLine("PtrReflection,       字段,指定类型（值类型无装箱,传参无拷贝）：" + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                EmitWrapperTypeFieldAndProperty valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass,int>)valueWrap.setValueDelegate)(obj, 13);
                v1 = ((Func<MyClass,int>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.str);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass, string>)valueWrap.setValueDelegate)(obj, "a4d2");
                v2 = ((Func<MyClass, string>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.point);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass, Vector3>)valueWrap.setValueDelegate)(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = ((Func<MyClass, Vector3>)valueWrap.getValueDelegate)(obj);
            }
            oTime.Stop();
            Console.WriteLine("Emit,                字段, 指定类型（值类型无装箱）：          " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.one);
                var valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, int>)valueWrap.setValueDelegate)(obj, 13);
                v1 = ((Func<MyClass, int>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.str);
                valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, string>)valueWrap.setValueDelegate)(obj, "a4d2");
                v2 = ((Func<MyClass, string>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.point);
                valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, Vector3>)valueWrap.setValueDelegate)(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = ((Func<MyClass, Vector3>)valueWrap.getValueDelegate)(obj);
            }
            oTime.Stop();
            Console.WriteLine("Expression,          字段, 指定类型（值类型无装箱）：          " + oTime.Elapsed.TotalMilliseconds + " 毫秒");
            Console.WriteLine();


            //指针反射
            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                ObjReference objReference = new ObjReference(obj);
                void** handleVoid = (void**)GeneralTool.AsPointer<ObjReference>(ref objReference);
                byte** handleByte = (byte**)handleVoid;

                string name = nameof(obj.One);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.propertyDelegateItem.setInt32(*handleVoid, 18);
                    v1 = typeAddr.propertyDelegateItem.getInt32(*handleVoid);
                }

                name = nameof(obj.Str);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);
                    typeAddr.propertyDelegateItem.setString(*handleVoid, "sa32");
                    v2 = typeAddr.propertyDelegateItem.getString(*handleVoid);
                }

                name = nameof(obj.Point);
                fixed (char* p = name)
                {
                    TypeAddrFieldAndProperty typeAddr = reflectionWrapper.Find(p, name.Length);

                    //typeAddr.propertyDelegateItem.setObject(*handleVoid, new Vector3(1.1f, 2.2f, 3.3f));
                    //v3 = (Vector3)typeAddr.propertyDelegateItem.getObject(*handleVoid);

                    ((Action<MyClass, Vector3>)typeAddr.propertyDelegateItem.setValueSouse)(obj, new Vector3(1.1f, 2.2f, 3.3f));
                    v3 = ((Func<MyClass, Vector3>)typeAddr.propertyDelegateItem.getValueSouse)(obj);
                }
            }
            oTime.Stop();
            Console.WriteLine("PtrReflection,       属性,指定类型（值类型无装箱）：           " + oTime.Elapsed.TotalMilliseconds + " 毫秒");

            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.One);
                EmitWrapperTypeFieldAndProperty valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass, int>)valueWrap.setValueDelegate)(obj, 13);
                v1 = ((Func<MyClass, int>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.Str);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass, string>)valueWrap.setValueDelegate)(obj, "sad2");
                v2 = ((Func<MyClass, string>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.Point);
                valueWrap = emitWrapperType.GetEmitWarp(name);
                ((Action<MyClass, Vector3>)valueWrap.setValueDelegate)(obj, new Vector3(1.1f, 2.2f, 3.3f));
                v3 = ((Func<MyClass, Vector3>)valueWrap.getValueDelegate)(obj);
            }
            oTime.Stop();
            Console.WriteLine("Emit,                属性, 指定类型（值类型无装箱）：          " + oTime.Elapsed.TotalMilliseconds + " 毫秒");


            oTime.Reset(); oTime.Start();
            for (int d = 0; d < testCount; d++)
            {
                string name = nameof(obj.One);
                var valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, int>)valueWrap.setValueDelegate)(obj, -3);
                v1 = ((Func<MyClass, int>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.Str);
                valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, string>)valueWrap.setValueDelegate)(obj, "a4d2");
                v2 = ((Func<MyClass, string>)valueWrap.getValueDelegate)(obj);

                name = nameof(obj.Point);
                valueWrap = expressionWrapperType.GetWarp(name);
                ((Action<MyClass, Vector3>)valueWrap.setValueDelegate)(obj, new Vector3(-1.1f, 2.2f, -3.3f));
                v3 = ((Func<MyClass, Vector3>)valueWrap.getValueDelegate)(obj);
            }
            oTime.Stop();
            Console.WriteLine("Expression,          属性, 指定类型（值类型无装箱）：          " + oTime.Elapsed.TotalMilliseconds + " 毫秒");



            Console.Read();
        }
    }
}
