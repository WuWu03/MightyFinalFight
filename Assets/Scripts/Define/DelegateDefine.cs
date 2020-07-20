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

public delegate int IntNotPar();
public delegate int IntParamT<T>(T t);
public delegate int IntParamT2<T1, T2>(T1 t1, T2 t2);

public delegate T TNotPar<T>();
public delegate T TParamT<T,P>(P P);
public delegate T TParamT2<T, P1, P2>(P1 p1, P2 p2);