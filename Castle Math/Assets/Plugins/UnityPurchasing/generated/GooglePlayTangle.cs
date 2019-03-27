#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("u9ajvu3v8IFEl463B7XSrgG6tguqlJfgKpabCrrxkKAIUTqxVF7lQ9xu7c7c4erlxmqkahvh7e3t6ezvg1bR3f0lU4D99N0fLaUCrtxtUJj8edTxn7jJBIulCxUSv6I6EyabnqmFS+Es0e+4P7piuEnEY+becmC5VeMF9AWSz+piyf+jYb7GfLprYb5u7ePs3G7t5u5u7e3sbpgeaF83r3YfIquFe6mZ6+Or8wgkJaZ5l1ojujgIB0ijLzFoFgAEa/nOV9G3gURv27Zeg9Cw02cncTEsTAcpZFxbyQ0gqweG5Ydm6nyt0+I2OWUk6D+DXeySDkzDTPHOvFwtB91hPzMsR1n6ykfGZcgaBrDrL3iLOpWuVx1nq3VsfGUWibGW3e7v7ezt");
        private static int[] order = new int[] { 6,5,6,10,9,11,7,11,13,10,10,13,13,13,14 };
        private static int key = 236;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
