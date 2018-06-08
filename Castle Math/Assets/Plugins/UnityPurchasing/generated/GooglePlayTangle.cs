#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("ZVXYWfpXhZkvdLDnFKUKMciC+DQ2GtR+s05wJ6Al/SfWW/x5Qe3/JsJzDZHTXNNuUSPDsphC/qCss9jGNQsIf7UJBJUlbg8/l86lLsvBetxD8XJRQ351eln1O/WEfnJycnZzcMp8mmuaDVB1/VZgPP4hWeMl9P4hY+ZLbgAnVpsUOpSKjSA9pYy5BAEcyU5CYrrMH2JrQoCyOp0xQ/LPB/FyfHND8XJ5cfFycnPxB4H3wKgw6YC9NBrkNgZ0fDRsl7u6OeYIxbySvzSYGXoY+XXjMkx9qab6u3egHPBEKcEcTy9M+LjurrPTmLb7w8RWJEk8IXJwbx7bCBEomCpNMZ4lKZQlp5eY1zywrveJn5v0ZlHITige2+rz4/qJFi4JQnFwcnNy");
        private static int[] order = new int[] { 12,11,2,5,12,7,9,10,11,13,11,12,13,13,14 };
        private static int key = 115;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
