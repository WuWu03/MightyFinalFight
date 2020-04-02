using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void VoidNotPar();
public delegate bool BoolNotPar();
public delegate void VoidParamT<T>(T t);
public delegate void VoidParamT2<T1, T2>(T1 t1, T2 t2);
public delegate bool BoolParamT<T>(T t);

