docker tag githubscraper deuclerio/githubscraper:latest

docker login

docker push deuclerio/githubscraper



heroku login

heroku container:login

docker tag githubscraper registry.heroku.com/githubscraper/web

docker push registry.heroku.com/githubscraper/web

heroku container:release web -a githubscraper

explorer "https://githubscraper.herokuapp.com/"