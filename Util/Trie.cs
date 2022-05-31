using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOnlineDatRipper.Util
{
    internal class Trie
    {
        protected readonly Dictionary<char, Trie> children = new Dictionary<char, Trie>();
        protected string content;
        protected bool terminal = false;       

        public Trie()
        {

        }

        private Trie(string content)
        {
            this.content = content;
        }

        protected void Add(char character)
        {
            string s;
            if (this.content == null)
            {
                s = Convert.ToString(character);
            }
            else
            {
                s = this.content + character;
            }
            children.Add(character, new Trie(s));
        }
        
        /// <summary>
        /// Put candidate into the list of possible auto-completes.
        /// </summary>
        /// <param name="key">key candidate</param>
        public void Insert(string key)
        {
            Trie node = this;
            foreach (char c in key.ToCharArray())
            {
                if (!node.children.ContainsKey(c))
                {
                    node.Add(c);
                }
                node = node.children[c];
            }
            node.terminal = true;
        }

        /// <summary>
        /// Finds node for the given key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>empty string if not exits othersie returns string</returns>
        public string Find(string key)
        {
            Trie node = this;
            foreach (char c in key.ToCharArray())
            {
                if (!node.children.ContainsKey(c))
                {
                    return string.Empty;
                }
                node = node.children[c];
            }
            return node.content;
        }
        
        /// <summary>
        /// Gives list of candidates for possible given prefix
        /// </summary>
        /// <param name="prefix"><given prefix/param>
        /// <returns>list of candidates(completes)</returns>
        public List<string> AutoComplete(string prefix)
        {
            Trie Trienode = this;
            foreach (char c in prefix.ToCharArray())
            {
                if (!Trienode.children.ContainsKey(c))
                {
                    return new List<string>();
                }
                Trienode = Trienode.children[c];
            }
            return Trienode.AllPrefixes();
        }

        protected List<string> AllPrefixes()
        {
            List<string> result = new List<string>();
            if (this.terminal)
            {
                result.Add(this.content);
            }
            foreach (var key in children.Keys)
            {
                Trie child = children[key];
                List<string> childPrefixes = child.AllPrefixes();
                result.AddRange(childPrefixes);
            }
            return result;
        }

        /// <summary>
        /// Clear the Tries.
        /// Keys need to be added again.
        /// </summary>
        public void Clear()
        {
            children.Clear();
        }

    }
}
