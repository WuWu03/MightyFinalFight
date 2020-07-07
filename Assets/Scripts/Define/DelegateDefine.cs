using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidNotPar();
public delegate void VoidParamT<T>(T t);
public delegate void VoidParamT2<T1, T2>(T1 t1, T2 t2);

public delegate bool BoolNotPar();
public delegate bool BoolParamT<T>(T t);
public delegate bool BoolParamT2<T1, T2>(T1 t1, T2 t2);

public delegate float FloatNotPar();
public delegate float FloatParamT<T>(T t);
public delegate float FloatParamT2<T1, T2>(T1 t1, T2 t2);
