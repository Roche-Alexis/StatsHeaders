# StatsHeaders

## Comment lancer le projet
Naviguer dans le répertoire Racine

Lancer Projet.sln pour ouvrir la solution sur Microsoft Visual Studio

Lancer le projet

Se connecter sur le site : http://localhost:8080/

Cliquer sur l'un des boutons pour lancer un scénario


## But du site
Ce site est un site web dynamique, qui lorsqu'on appuye sur un bouton, va envoyer cette information à notre serveur. Notre serveur dispose d'une liste d'urls, et il envoye une reqûete à chacune de ses urls, récupérer les headers et effectuer des statistiques dessus. 


## Question 1
Dans cette question, on s'intéresse au type de serveur utilisé par chaque site. On affiche le nombre d'occurences et le pourcentage par ordre décroissant des serveurs utisés.

## Question 2
Dans cette question, on utilise le champ "Last-Modified" pour analyser la date de dernière modification des pages web. On affiche la valeur moyenne, l'écart-type et la date de dernière modification pour chacune des pages.


## Question 3
Dans cette question, on s'intéresse à 3 propriétes des headers.

* Dans un premier cas, on regarde le nombre de champ utilisé par un header. On constate que cette valeur varie énormément d'un site à l'autre. On affiche également le champ le plus présent et celui le moins présent (ainsi que leur occurence), puis le nombre de champs par site.

* On s'intéresse à présent à la longeur de la réponse, qui nous est donnée avec le champ "Content-Length". On affiche la valeur moyenne, l'écart-type et la longeur pour chaque site.

* Enfin, on regarde à l'utilisation des cookies. On constate qu'un fort pourcentage de site utilise le champ "Cookies" des headers (70%). On affiche les sites utilisant les cookies, et leur contenu. 
