import numpy as np

class Checker:
    def __init__(self) -> None:
        self.size = 8

    def get_initial_state(self):
        return [[1, 0] * (self.size // 2),
                [0, 1] * (self.size // 2),
                [1, 0] * (self.size // 2),
                [0] * self.size,
                [0] * self.size,
                [0, -1] * (self.size // 2),
                [-1, 0] * (self.size // 2),
                [0, -1] * (self.size // 2)]
        
    def get_next_state(self, state, action, player):
        x1, y1, x2, y2 = action
        if abs(x2 - x1) == 1 and abs(y2 - y1) == 1:                      
            state[x2][y2] = state[x1][y1]
            if x2 == 7 * (player == 1):
                state[x2][y2] = 2 * player
            state[x1][y1] = 0
        elif abs(x2 - x1) == 2 and abs(y2 - y1) == 2:
            mid_x = (x1 + x2) // 2
            mid_y = (y1 + y2) // 2                            
            state[x2][y2] = state[x1][y1]
            if x2 == 7 * (player == 1):
                state[x2][y2] = 2 * player
            state[x1][y1] = 0
            state[mid_x][mid_y] = 0
            if state[x2][y2] > 0:
                if self.can_move(state, x2, y2, x2+2, y2+2):
                    state = self.get_next_state(state, (x2, y2, x2+2, y2+2), player)
                elif self.can_move(state, x2, y2, x2+2, y2-2):
                    state = self.get_next_state(state, (x2, y2, x2+2, y2-2), player)
                if state[x2][y2] == 2:
                    if self.can_move(state, x2, y2, x2-2, y2+2):
                        state = self.get_next_state(state, (x2, y2, x2-2, y2+2), player)
                    elif self.can_move(state, x2, y2, x2-2, y2-2):
                        state = self.get_next_state(state, (x2, y2, x2-2, y2-2), player)
            elif state[x2][y2] < 0:
                if self.can_move(state, x2, y2, x2-2, y2+2):
                    state = self.get_next_state(state, (x2, y2, x2-2, y2+2), player)
                elif self.can_move(state, x2, y2, x2-2, y2-2):
                    state = self.get_next_state(state, (x2, y2, x2-2, y2-2), player)
                if state[x2][y2] == 2:
                    if self.can_move(state, x2, y2, x2+2, y2+2):
                        state = self.get_next_state(state, (x2, y2, x2+2, y2+2), player)
                    elif self.can_move(state, x2, y2, x2+2, y2-2):
                        state = self.get_next_state(state, (x2, y2, x2+2, y2-2), player)
        return state

    def get_valid_moves(self, state, player):
        moves = []
        for i in range(self.size):
            for j in range(self.size):
                if state[i][j] in (player, player * 2):
                    if state[i][j] > 0:
                        if self.can_move(state, i, j, i+1, j+1):
                            moves.append((i, j, i+1, j+1))
                        if self.can_move(state, i, j, i+1, j-1):
                            moves.append((i, j, i+1, j-1))
                        if self.can_move(state, i, j, i+2, j+2):
                            moves.append((i, j, i+2, j+2))
                        if self.can_move(state, i, j, i+2, j-2):
                            moves.append((i, j, i+2, j-2))
                        if state[i][j] == 2:                            
                            if self.can_move(state, i, j, i-1, j+1):
                                moves.append((i, j, i-1, j+1))
                            if self.can_move(state, i, j, i-1, j-1):
                                moves.append((i, j, i-1, j-1))
                            if self.can_move(state, i, j, i-2, j+2):
                                moves.append((i, j, i-2, j+2))
                            if self.can_move(state, i, j, i-2, j-2):
                                moves.append((i, j, i-2, j-2))
                    elif state[i][j] < 0:                            
                        if self.can_move(state, i, j, i-1, j+1):
                            moves.append((i, j, i-1, j+1))
                        if self.can_move(state, i, j, i-1, j-1):
                            moves.append((i, j, i-1, j-1))
                        if self.can_move(state, i, j, i-2, j+2):
                            moves.append((i, j, i -2, j+2))
                        if self.can_move(state, i, j, i-2, j-2):
                            moves.append((i, j, i-2, j-2))
                        if state[i][j] == -2:
                            if self.can_move(state, i, j, i+1, j+1):
                                moves.append((i, j, i+1, j+1))
                            if self.can_move(state, i, j, i+1, j-1):
                                moves.append((i, j, i+1, j-1))
                            if self.can_move(state, i, j, i+2, j+2):
                                moves.append((i, j, i+2, j+2))
                            if self.can_move(state, i, j, i+2, j-2):
                                moves.append((i, j, i+2, j-2))

        return moves
    
    def can_move(self, state, x1, y1, x2, y2):
        if x2 < 0 or x2 > 7 or y2 < 0 or y2 > 7:
            return False
        if state[x2][y2] != 0:
            return False
        if abs(x2 - x1) == 1 and abs(y2 - y1) == 1:
            return True
        if abs(x2 - x1) == 2 and abs(y2 - y1) == 2:
            mid_x = (x1 + x2) // 2
            mid_y = (y1 + y2) // 2
            if state[mid_x][mid_y] * state[x1][y1] < 0:
                return True
        return False
    
    def check_win(self, state, player):
        if player > 0:
            return np.sum(np.array(state) < 0) == 0
        return np.sum(np.array(state) > 0) == 0
    
    def get_value_and_terminated(self, state, player):
        if self.check_win(state, -player):
            return 1, True
        if np.sum(self.get_valid_moves(state, player)) == 0:
            return 0, True
        return 0, False
    
    def get_opponent(self, player):
        return -player
    
    def get_opponent_value(self, value):
        return -value

    def print_board(self, state):
        for i in range(self.size):
            print(state[i])

# checker = Checker()
# player = 1
# state = checker.get_initial_state()

# while True:
#     checker.print_board(state)
#     valid_moves = checker.get_valid_moves(state, player)
#     print('Valid moves:')
#     for i, move in enumerate(valid_moves):
#         print(i, "-", move)
#     action = int(input(f"Player {player}:"))

#     if action >= len(valid_moves):
#         print("Action not valid")
#         continue

#     state = checker.get_next_state(state, valid_moves[action], player)

#     value, is_terminal = checker.get_value_and_terminated(state, valid_moves[action], player)

#     if is_terminal:
#         print('End game!!!')
#         checker.print_board(state)
#         if value == 1:
#             print(player, "won")
#         else:
#             print("draw")
#         break    

#     print('------------')
#     player = checker.get_opponent(player)