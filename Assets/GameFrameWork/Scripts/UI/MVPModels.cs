using System.Collections.Generic;

namespace GameFrameWork.UI
{
    public static class MVPModels
    {
        private static HashSet<BaseModel> s_UIModels = new ();

        public static bool RegistModel(BaseModel model)
        {
            return s_UIModels.Add(model);
        }

        public static bool UnRegistModel(BaseModel model)
        {
            return s_UIModels.Remove(model);
        }

        public static T GetModel<T>() where T : BaseModel
        {
            foreach (var model in s_UIModels)
            {
                if (model is T tModel)
                {
                    return tModel;
                }
            }

            return null;
        }
    }
}
