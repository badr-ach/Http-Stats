# Solution pour lancer 4 serveurs HTTPS simultanément

Cette solution en C# permet de lancer quatre serveurs HTTPS qui écoutent sur les ports 8000, 8080, 8081 et 8082. 
Le serveur sur le port 8000 est responsable de renvoyer la page HTML. Pour utiliser ce projet c#, il suffit de lancer la solution en visual studio et de démarrer les quatre projets en même temps en allant dans les paramètres de la solution et en spécifiant que les quatre doivent être démarrés.

## Utilisation

1. Clonez ce dépôt sur votre machine locale.
2. Ouvrez le projet dans un environnement de développement intégré compatible avec C#, tel que Visual Studio ou Visual Studio Code.
3. Lancez la solution et démarrer les quatre projets simultanément en allant dans les paramètres de la solution et en spécifiant que les quatre doivent être démarrés.
4. Une fois les serveurs lancés, vous pouvez accéder à l'application en ouvrant un navigateur Web et en accédant aux URL suivantes :
- https://localhost:8000/ : qui renvoit la page html 
- https://localhost:8080/ : qui renvoit le json de la question 1
- https://localhost:8081/ : qui renvoit le json de la question 2
- https://localhost:8082/ : qui renvoit le json de la question 3

## Configuration

Il y a également un fichier 'urls.txt' à la racine du projet/solution où il faut mettre les URL sur lesquelles on veut des stats. 

## Headers pour les stats

Il y a trois headers spécifiques que nous avons choisi pour renvoyer les statistiques :
- `x-status-cache` : pour voir combien de fois nous avons accédé au cache proprement
- `set-cookies` : pour savoir quels sont les champs de cookies les plus utilisés/requis
- `vary` : pour savoir quels sont les headers les plus utilisés pour répondre à nos requêtes.
