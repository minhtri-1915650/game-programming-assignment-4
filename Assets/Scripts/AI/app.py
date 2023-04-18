import random
from flask import Flask, request, jsonify
from MCTS import MCTS
from Checker import Checker

checker = Checker()
mcts = MCTS(checker)

app = Flask(__name__)

@app.put("/checker-agent/mcts-move")
def get_mcts_move():
    content_type = request.headers.get('Content-Type')
    if 'application/json' in content_type:
        json = request.json

        state = json['state']
        player = json['player']
        num_searches = json['num_searches']
        
        moves = mcts.search(state, player, num_searches)
        action = max(moves, key=moves.get)

        return jsonify(action)
    else:
        print(content_type)
        return {"error": "Content-Type not supported!"}, 415
    
@app.put("/checker-agent/random-move")
def get_random_move():
    content_type = request.headers.get('Content-Type')
    if 'application/json' in content_type:
        json = request.json

        state = json['state']
        player = json['player']

        moves = checker.get_valid_moves(state, player)
        action = random.choice(moves)

        return jsonify(action)
    else:
        print(content_type)
        return {"error": "Content-Type not supported!"}, 415
