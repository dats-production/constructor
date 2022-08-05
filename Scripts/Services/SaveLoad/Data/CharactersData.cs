using System;
using System.Collections.Generic;

namespace Services.SaveLoad.Data
{
    [Serializable]
    public class CharactersData
    {
        public List<CharacterData> Characters;

        public CharactersData(List<CharacterData> characters)
        {
            Characters = characters;
        }
    }
}