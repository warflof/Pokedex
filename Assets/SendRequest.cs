using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;



public class SendRequest : MonoBehaviour
{
    public int pokeNumber;
    public string apiUrl;
    public Image pokemonSprite;
    public TextMeshProUGUI title;
    public TextMeshProUGUI iD;
    public TextMeshProUGUI poids;
    public TextMeshProUGUI taille;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetPokemon());
        StartCoroutine(GetPokemonImage());
        
    }
    public void Update()
    {
        apiUrl = "https://pokeapi.co/api/v2/pokemon/" + pokeNumber + "/";
        
    }

    public void NextPokemon()
    {
        pokeNumber++;
        StartCoroutine(GetPokemon());
        StartCoroutine(GetPokemonImage());
    }

    public void PreviousPokemon()
    {
        pokeNumber--;
        StartCoroutine(GetPokemon());
        StartCoroutine(GetPokemonImage());
    }

    public string CapitalizeFirstLetter(string str)
    {
        return char.ToUpper(str[0]) + str.Substring(1);
    }

    private IEnumerator GetPokemon()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
            
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Pokemon pokemon = JsonUtility.FromJson<Pokemon>(request.downloadHandler.text);
                string pokeName = CapitalizeFirstLetter(pokemon.name);
                int pokePoids = int.Parse(pokemon.weight) / 10;
                int pokeTaille = int.Parse(pokemon.height) * 10;
                title.text = pokeName;
                iD.text = "ID : " + pokemon.id;
                poids.text = "Poids : " + pokePoids.ToString() + "Kg";
                taille.text = "Taille : " + pokeTaille.ToString() + "cm";
                Debug.Log("Mon pokémon est : " + pokemon.name + ", il fait " + (int.Parse(pokemon.height) * 10) + " cm. Et il pèse " + (int.Parse(pokemon.weight) / 10) + " kg");
               
            } 
            else
            {
                Debug.LogError("Une erreur est survenue");
            }
        }

    }

    private IEnumerator GetPokemonImage()
    {
        using (UnityWebRequest img = UnityWebRequest.Get(apiUrl))
        {
            yield return img.SendWebRequest();

            if (img.result == UnityWebRequest.Result.Success)
            {
                PokemonData pokemonData = JsonUtility.FromJson<PokemonData>(img.downloadHandler.text);

                string frontDefault = pokemonData.sprites.other.home.front_default;
                string frontFemale = pokemonData.sprites.front_female;
                // ... et ainsi de suite pour les autres clés de "sprites"

                Debug.Log("Front Default: " + frontDefault);
                Debug.Log("Front Female: " + frontFemale);

                StartCoroutine(LoadSpriteFromUrl(frontDefault, (loadedSprite) =>
                {
                    // Utilisez le Sprite chargé ici, par exemple l'affecter à un SpriteRenderer
                    pokemonSprite.sprite = loadedSprite;
                }));
                
                
            }
        }
    }

    IEnumerator LoadSpriteFromUrl(string url, System.Action<Sprite> onSpriteLoaded)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            onSpriteLoaded(sprite);
        }
        else
        {
            Debug.LogError("Erreur lors du chargement de l'image : " + request.error);
        }
    }
}
[System.Serializable]
public class Pokemon
{
    public string name;
    public string id;
    public string height;
    public string weight;
    public string sprites;
}


[System.Serializable]
public class PokemonData
{
    public SpritesData sprites;
}

[System.Serializable]
public class SpritesData
{
    public OtherData other;
    public string front_default;
    public string front_female;
    
    // ... et ainsi de suite pour les autres clés de "sprites"
}
[System.Serializable]
public class OtherData
{
    public DreamWorld home;
}

[System.Serializable]
public class DreamWorld
{
    public string front_default;
}

