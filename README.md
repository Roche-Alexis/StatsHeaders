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
Dans cette question, nous avons choisi de regarder les statistiques liées à trois propriétés des headers : le nombre de champs utilisés par un header, la longueur de la réponse et l'utilisation des cookies.

* En ce qui concerne le nombre de champs utilisés, nous avons remarqué que cette valeur varie énormément d'un site à l'autre, et nous avons donc décidé d'afficher également le champ le plus présent et celui le moins présent (ainsi que leur occurrence) ainsi que le nombre de champs par site.

* En ce qui concerne la longueur de la réponse, nous avons choisi d'afficher la valeur moyenne, l'écart-type et la longueur pour chaque site. Cette statistique peut nous donner une idée de la taille des pages web et nous permettre de comprendre comment les sites web sont construits.

* Enfin, nous avons étudié l'utilisation des cookies et nous avons constaté que 70% des sites utilisaient le champ "Cookies" des headers. Nous avons affiché les sites utilisant les cookies et leur contenu pour permettre aux utilisateurs de mieux comprendre comment les cookies sont utilisés pour suivre leur activité en ligne.
