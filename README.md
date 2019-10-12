Gathering Chess
===============

Simple chess game that I'm building to demonstrate features of Unisave.


### Behaviours needed

- select a deck and play with random opponent or transparent AI
    - get coins for winning (or quests or sth...)
- participate in battles
    - for gems / pieces / status
    - one to one
    - pay to enter a battle, win the sum
- participate in tournaments `(later feature)`
    - for gems / pieces / status
    - many competitors, you see the whole tournament tree
- build decks from owned pieces
- customize profile look (nickname, avatar)
- buy pieces or some booster packs
- buy gems, buy coins for gems
- select a deck and go practising locally
    - no reward coz it cannot be synced with the server
- manage device accounts, add recoverable login methods
- find friends, chat with them, go play matches with them `(later feature)`


### Data

- player
    - nickname, not unique
    - number (e.g. `#40125`), unique, used to find friends, invisible to others
    - avatar / icon (one of few to choose)
    - pieces
    - decks
- deck
    - name
    - title piece (thumbnail)
- piece
    - image


### Player performance metrics

- percentage of won matches
- average thinking time


### UI structure

- main menu (launch screen)
    - account management (icon in left upper corner)
    - coins and gems counter
    - nickname + avatar display (+ click to modify)
        - screen where you can modify this (modal or sth...)
    - play online button
        - deck selection widget
        - play button
    - my collection button
        - buy gems / booster packs
    - practise offline button
