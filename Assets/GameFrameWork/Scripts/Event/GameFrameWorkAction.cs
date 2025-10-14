namespace GameFrameWork.Event
{
    /// <summary>
    /// 无参GameFrameWorkBooleanAction
    /// </summary>
    public delegate void GameFrameWorkAction();
    public delegate bool GameFrameWorkBooleanAction();
    public delegate float GameFrameWorkFloatAction();
    public delegate int GameFrameWorkIntAction();
    public delegate string GameFrameWorkStringAction();
    public delegate byte GameFrameWorkByteAction();
    public delegate TResult GameFrameWorkTemplateAction<TResult>();

    /// <summary>
    /// 一个参数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    public delegate void GameFrameWorkAction<in T>(T t);
    public delegate bool GameFrameWorkBooleanAction<in T>(T t);
    public delegate float GameFrameWorkFloatAction<in T>(T t);
    public delegate int GameFrameWorkIntAction<in T>(T t);
    public delegate string GameFrameWorkStringAction<in T>(T t);
    public delegate byte GameFrameWorkByteAction<in T>(T t);
    public delegate TResult GameFrameWorkTemplateAction<in T, out TResult>(T t);

    /// <summary>
    /// 两个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    public delegate void GameFrameWorkAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate float GameFrameWorkFloatAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate int GameFrameWorkIntAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate string GameFrameWorkStringAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate byte GameFrameWorkByteAction<in T1, in T2>(T1 t1, T2 t2);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, out TResult>(T1 t1, T2 t2);

    /// <summary>
    /// 三个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    public delegate void GameFrameWorkAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate float GameFrameWorkFloatAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate int GameFrameWorkIntAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate string GameFrameWorkStringAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate byte GameFrameWorkByteAction<in T1, in T2, in T3>(T1 t1, T2 t2, T3 t3);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, in T3, out TResult>(T1 t1, T2 t2, T3 t3);

    /// <summary>
    /// 四个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    public delegate void GameFrameWorkAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate float GameFrameWorkFloatAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate int GameFrameWorkIntAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate string GameFrameWorkStringAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate byte GameFrameWorkByteAction<in T1, in T2, in T3, in T4>(T1 t1, T2 t2, T3 t3, T4 t4);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, in T3, in T4, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4);

    /// <summary>
    /// 五个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    /// <param name="t5"></param>
    public delegate void GameFrameWorkAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate float GameFrameWorkFloatAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate int GameFrameWorkIntAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate string GameFrameWorkStringAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate byte GameFrameWorkByteAction<in T1, in T2, in T3, in T4, in T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5);

    /// <summary>
    /// 六个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    /// <param name="t5"></param>
    /// <param name="t6"></param>
    public delegate void GameFrameWorkAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate float GameFrameWorkFloatActio<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate int GameFrameWorkIntAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate string GameFrameWorkStringAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate byte GameFrameWorkByteAction<in T1, in T2, in T3, in T4, in T5, in T6>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6);

    /// <summary>
    /// 七个参数
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <typeparam name="T3"></typeparam>
    /// <typeparam name="T4"></typeparam>
    /// <typeparam name="T5"></typeparam>
    /// <typeparam name="T6"></typeparam>
    /// <typeparam name="T7"></typeparam>
    /// <param name="t1"></param>
    /// <param name="t2"></param>
    /// <param name="t3"></param>
    /// <param name="t4"></param>
    /// <param name="t5"></param>
    /// <param name="t6"></param>
    /// <param name="t7"></param>
    public delegate void GameFrameWorkAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate bool GameFrameWorkBooleanAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate float GameFrameWorkFloatActio<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate int GameFrameWorkIntAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate string GameFrameWorkStringAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate byte GameFrameWorkByteAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
    public delegate TResult GameFrameWorkTemplateAction<in T1, in T2, in T3, in T4, in T5, in T6, in T7, out TResult>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5, T6 t6, T7 t7);
}