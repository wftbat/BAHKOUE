# Évaluation de Fin de Semestre: Trading Algorithmique avec Lean

## Introduction

L'objectif de cette évaluation est de développer un algorithme de trading en utilisant la plateforme Lean. Vous travaillerez en groupes de 3 personnes. L'évaluation finale prendra en compte la qualité de votre algorithme, la documentation associée, la structure de votre code et la présentation finale.

## Détails de la Tâche

1. **Formation des Équipes**:
   - Formez des équipes de 3 membres.
   - Une fois les équipes formées, choisissez un nom pour votre équipe, et  inscrivez-le sur le document correspondant à votre classe :
     - [Inscriptions pour le groupe 1](https://docs.google.com/spreadsheets/d/1KBHn4QDCMmkiWtkpTGSLgjkgAjoj4SEB/edit?usp=sharing&ouid=113270325295686005611&rtpof=true&sd=true)
     - [Inscriptions pour le groupe 2](https://docs.google.com/spreadsheets/d/1o1Mh6gHBxKG2i1J4C9u8iXQqAW3cC3vS/edit?usp=sharing&ouid=113270325295686005611&rtpof=true&sd=true)
     
     Vous utiliserez votre nom d'équipe dans la dénomination des fichiers ou des répertoires que vous ajouterez au dépôt
   - Chaque équipe doit développer un algorithme de trading unique avec Lean. S'inspirer des 500+ exemples fournis par Lean est permis, mais reproduire un exemple existant ne sera pas accepté.


2. **Fork et Travail**:
   - Chaque groupe doit créer un fork du dépôt : [https://github.com/MyIntelligenceAgency/Lean](https://github.com/MyIntelligenceAgency/Lean).
   - Tous les développements doivent être effectués sur votre fork.

3. **Environnement de Développement**: 
   - Nous travaillons hors ligne avec le moteur Lean brut du dépôt GitHub. 
   - Profitez de cette configuration pour explorer des opportunités d'innovation et d'adaptation.

4. **Source des Données**:
   - Utilisez le convertisseur fourni pour construire des données de backtesting à partir de dumps de trades de cryptomonnaies provenant de [BitcoinCharts](https://api.bitcoincharts.com/v1/csv/).
   - Ces données seront la base principale de vos backtests.

5. **Exigences de Soumission**:
   - Votre algorithme doit être accompagné de:
     - Un rapport détaillé décrivant la stratégie, les hypothèses et les résultats.
     - Documentation du code.
     - Résultats du backtest avec des métriques pertinentes.
    
6. **Pull Requests (PRs)**:
   - Une fois votre travail terminé, soumettez une ou plusieurs PRs vers le dépôt original [https://github.com/MyIntelligenceAgency/Lean](https://github.com/MyIntelligenceAgency/Lean).
   - Assurez-vous que le titre et la description de votre PR soient clairs et détaillés.

7. **Présentation Finale**:
   - Préparez une présentation de 15 minutes pour le dernier jour de cours.
   - Cette présentation doit couvrir :
     - Introduction et objectif de votre algorithme.
     - Stratégie adoptée et raisonnement.
     - Défis rencontrés et comment ils ont été surmontés.
     - Démonstration de l'algorithme.
     - Résultats obtenus.
   - Supportez votre présentation avec des slides clairs et concis.

## Critères d'Évaluation

- **Originalité** : Votre algorithme doit être original.
- **Complexité** : Profondeur de la recherche et sophistication de la stratégie.
- **Performance** : Résultats de backtest.
- **Utilisation des Options Avancées** : Incorporation de frameworks, d'optimisation ou d'apprentissage automatique.
- **Collaboration en Équipe** : Organisation, répartition des tâches, communication.
- **Présentation Finale** : Clarté, structure, capacité à communiquer les idées.


## Suggestions Avancées

Pour donner à vos projets un avantage distinctif et potentiellement obtenir une meilleure note, envisagez d'explorer les options avancées et approfondies suivantes:

1. **Framework de Haut Niveau de Lean**: Plutôt que de passer des ordres explicitement, adoptez le framework de haut niveau de Lean. Cette méthode vous permet de définir des objectifs de portefeuille basés sur les Alpha/insights de votre algorithme.

2. **Gestion du Risque**: Comprenez et intégrez activement des techniques de gestion des risques dans votre stratégie. Examinez des mesures comme le ratio de Sharpe, les drawdowns maximaux, et autres métriques pour évaluer le risque par rapport au rendement.

3. **Optimisation & Prévention de la Sur-Optimisation**: Tout en utilisant le lanceur d'optimisation de Lean pour ajuster les hyperparamètres, soyez conscient du risque de sur-optimisation. Assurez-vous que votre algorithme n'est pas trop bien ajusté aux données historiques.

4. **Adaptabilité & Événements de Marché**: Réfléchissez à la manière dont votre algorithme peut s'adapter aux changements du marché et comment il gérerait de grands événements du marché ou des anomalies.

5. **Apprentissage Automatique**: Envisagez d'intégrer des techniques d'apprentissage automatique. Lean offre des exemples avec des Machines à Vecteurs de Support (SVM) via Accord.Net. Cela peut ajouter une robustesse et une adaptabilité à votre stratégie.

6. **Benchmarking**: Comparez les performances de votre algorithme à un benchmark pertinent pour mettre en évidence ses avantages et ses défis.

7. **Tests hors échantillon**: Séparez vos données en ensembles d'apprentissage et de test pour évaluer les performances de votre algorithme dans des conditions plus proches du monde réel.

8. **Interactions avec l'Ecosystème Lean**: Utilisez les fonctionnalités supplémentaires de Lean, telles que les notifications et la journalisation, pour améliorer la traçabilité et la compréhension de votre algorithme.

9. **Aspects Fondamentaux & Techniques**: Intégrez des données non seulement techniques, mais aussi fondamentales, pour influencer vos décisions de trading. Pensez aux actualités, aux indicateurs macroéconomiques, et à d'autres sources d'information pertinentes.


## Option Alternative

Pour ceux qui souhaitent contribuer à notre environnement de classe et sont à l'aise avec le développement en C#, vous avez la possibilité de travailler pour améliorer notre configuration de développement. Cela pourrait impliquer d'améliorer le convertisseur, d'intégrer davantage de sources de données, ou de corriger d'éventuels problèmes dans notre configuration actuelle.

De telles contributions, fournies sous forme de pull-requests, seront évaluées très favorablement indépendament de l'agorithme associé.

## Conclusion

L'expérience d'apprentissage est aussi précieuse que le résultat final. Nous encourageons la collaboration, l'exploration, et l'innovation.

Bonne chance à tous!
