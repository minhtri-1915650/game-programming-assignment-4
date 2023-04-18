from Checker import Checker
import numpy as np
import math

class Node:
    def __init__(self, game: Checker, state, player, parent=None, action_taken=None) -> None:
        self.game = game
        self.state = state
        self.player = player
        self.parent = parent
        self.action_taken = action_taken
                
        self.children = []
        self.expandable_moves = game.get_valid_moves(state, player)
        
        self.visit_count = 0
        self.value_sum = 0

    def is_fully_expanded(self):
        return np.sum(self.expandable_moves) == 0 and len(self.children) > 0
    
    def select(self):
        best_child = None
        best_ucb = -np.inf
        
        for child in self.children:
            ucb = self.get_ucb(child)
            if ucb > best_ucb:
                best_child = child
                best_ucb = ucb
                
        return best_child
    
    def get_ucb(self, child):
        q_value = 1 - ((child.value_sum / child.visit_count) + 1) / 2
        return q_value + math.sqrt(2 * math.log(self.visit_count) / child.visit_count)
    
    def expand(self):
        idx = np.random.choice(range(len(self.expandable_moves)))
        action = self.expandable_moves.pop(idx)
        
        child_state = [x[:] for x in self.state]
        child_state = self.game.get_next_state(child_state, action, self.player)
        
        child = Node(self.game, child_state, -self.player, self, action)
        self.children.append(child)
        return child
    
    def simulate(self, target_player):
        _, is_terminal = self.game.get_value_and_terminated(self.state, self.player)
        
        if is_terminal:
            value = 0 if self.player == target_player else 1
            return value
        
        rollout_state = [x[:] for x in self.state]
        rollout_player = self.player
        while True:
            valid_moves = self.game.get_valid_moves(rollout_state, rollout_player)
            idx = np.random.choice(range(len(valid_moves)))
            action = valid_moves[idx]
            rollout_state = self.game.get_next_state(rollout_state, action, rollout_player)            
            rollout_player = self.game.get_opponent(rollout_player)
            _, is_terminal = self.game.get_value_and_terminated(rollout_state, rollout_player)
            if is_terminal:
                value = 0 if rollout_player == target_player else 1
                return value    
            
    def backpropagate(self, value):
        self.value_sum += value
        self.visit_count += 1
        
        value = self.game.get_opponent_value(value)
        if self.parent is not None:
            self.parent.backpropagate(value)

class MCTS:
    def __init__(self, game: Checker):
        self.game = game
        
    def search(self, state, player, num_searches):
        root = Node(self.game, state, player)
        for _ in range(num_searches):
            node = root
            
            while node.is_fully_expanded():
                node = node.select()
                
            _, is_terminal = self.game.get_value_and_terminated(node.state, node.player)
            value = 0 if node.player == player else 1
            
            if not is_terminal:
                node = node.expand()
                value = node.simulate(player)

            node.backpropagate(value)                
            
        actions = {}
        # print("hello:", root.action_taken)
        # print("by:", root.value_sum)
        # print("len:", len(root.children))
        for child in root.children:
            actions[child.action_taken] = child.visit_count
        return actions
    
# checker = Checker()
# state = checker.get_initial_state()
# player = 1
# mcts = MCTS(checker)

# print("Initializing board...")
# checker.print_board(state)
# print("Game start!!!")
# print("----------------")

# while True:
#     checker.print_board(state)
#     if player == 1:
#         print("Your turn")
#         valid_moves = checker.get_valid_moves(state, player)        
#         print('Valid moves:')
#         for i, move in enumerate(valid_moves):
#             print(i, "-", move)
#         action = int(input(f"Choose your move: "))

#         if action >= len(valid_moves):
#             print("Action not valid! Please try again")
#             continue

#         action = valid_moves[action]
            
#     else:
#         print("AI turn")
#         mcts_probs = mcts.search(state, player, 100)
#         action = max(mcts_probs, key=mcts_probs.get)
#         print("AI move:", action)
    
#     state = checker.get_next_state(state, action, player)
#     player = checker.get_opponent(player)
#     v, is_terminal = checker.get_value_and_terminated(state, player)
    
#     print("You {} - {} AI".format(np.sum(np.array(state) > 0), np.sum(np.array(state) < 0)))

#     if is_terminal:
#         print("Game over!!!")
#         checker.print_board(state)
#         if v == 1:
#             if player == 1:
#                 print("AI win!!!")
#             else:
#                 print("You win!!!")
#         else:
#             if player == -1:
#                 print("AI out of move!!!")
#             else:
#                 print("You out of move!!!")
#         break

#     print("----------------")