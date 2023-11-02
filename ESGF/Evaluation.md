# Évaluation de Fin de Semestre: Trading Algorithmique avec Lean

## Introduction

L'objectif de cette évaluation est de développer un algorithme de trading en utilisant la plateforme Lean. Vous travaillerez en groupes de 3 personnes. L'évaluation finale prendra en compte la qualité de votre algorithme, la documentation associée, la structure de votre code et la présentation finale.

## Détails de la Tâche

1. **Formation des Équipes**: Formez des équipes de 3 membres. Chaque équipe doit développer un algorithme de trading unique avec Lean.

2. **Fork et Travail**:
   - Chaque groupe doit créer un fork du dépôt : [https://github.com/MyIntelligenceAgency/Lean](https://github.com/MyIntelligenceAgency/Lean).
   - Tous les développements doivent être effectués sur votre fork.

3. **Environnement de Développement**: N'oubliez pas, nous travaillons hors ligne avec le moteur Lean brut du dépôt GitHub. Bien que cette configuration ait ses limites, elle offre également une pléthore d'opportunités d'innovation et d'adaptation.

4. **Source des Données**: Nous avons fourni un convertisseur pour vous aider à construire des données de backtesting à partir de dumps de trades de cryptomonnaies provenant de [BitcoinCharts](https://api.bitcoincharts.com/v1/csv/). Cette donnée sera la base principale de vos backtests.

5. **Exigences de Soumission**: Votre algorithme doit être accompagné de:
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
     - Démonstration de l'algorithme (si possible).
     - Résultats obtenus.
   - Supportez votre présentation avec des slides clairs et concis.

## Critères d'Évaluation

1. **Qualité de l'Algorithme** : Fonctionnement, originalité, complexité.
2. **Documentation** : Clarté, détails, structure.
3. **Collaboration en Équipe** : Organisation, répartition des tâches, communication.
4. **Présentation Finale** : Clarté, structure, capacité à communiquer les idées.

Pour donner à vos projets un avantage distinctif et potentiellement obtenir une meilleure note, envisagez d'explorer les options avancées suivantes fournies par Lean :

1. **Framework de Haut Niveau de Lean**: Au lieu de passer des ordres explicitement, utilisez le framework de haut niveau de Lean. Cette approche vous permet de définir des objectifs de portefeuille basés sur les Alpha/insights de votre algorithme.
   
2. **Lanceur d'Optimisation**: Utilisez le lanceur d'optimisation de Lean pour affiner les hyperparamètres de votre algorithme. Un algorithme bien ajusté peut souvent surpasser ses homologues.
   
3. **Apprentissage Automatique**: Intégrez l'apprentissage automatique à votre stratégie. Lean fournit des exemples utilisant des Machines à Vecteurs de Support (SVM) avec Accord.Net. Incorporer l'apprentissage automatique peut donner à votre stratégie de trading une robustesse et une adaptabilité que les algorithmes traditionnels pourraient ne pas avoir.

## Critères d'Évaluation

- **Originalité**: Votre algorithme doit être original et non une réplique directe des 500+ exemples fournis par Lean.
- **Complexité**: Nous apprécions la profondeur de la recherche et la complexité de votre stratégie.
- **Performance**: Vos résultats de backtest, bien que non le seul critère, joueront un rôle dans votre évaluation.
- **Utilisation des Options Avancées**: Comme mentionné ci-dessus, l'intégration de frameworks de haut niveau, d'optimisation, ou d'apprentissage automatique sera vue favorablement.

## Option Alternative

Pour ceux qui souhaitent contribuer à notre environnement de classe et sont à l'aise avec le développement en C#, vous avez la possibilité de travailler pour améliorer notre configuration de développement. Cela pourrait impliquer d'améliorer le convertisseur, d'intégrer davantage de sources de données, ou de corriger d'éventuels problèmes dans notre configuration actuelle.

## Conclusion

Quel que soit le chemin que vous choisissez, n'oubliez pas que le voyage et l'expérience d'apprentissage sont aussi précieux que le résultat final. Nous encourageons la collaboration, l'exploration, et l'innovation.

Bonne chance à tous!
