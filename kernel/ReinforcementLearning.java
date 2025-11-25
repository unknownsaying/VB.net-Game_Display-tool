import java.util.*;
import java.util.concurrent.ThreadLocalRandom;
import java.io.Serializable;

/**
 * Comprehensive Reinforcement Learning implementation in Java
 * Demonstrates Q-Learning algorithm with multiple environments
 */
public class ReinforcementLearning {
    
    // MARK: - Core RL Types and Interfaces
    
    @FunctionalInterface
    public interface StateActionFunction<S, A> {
        double apply(S state, A action);
    }
    
    @FunctionalInterface
    public interface Policy<S, A> {
        A chooseAction(S state);
    }
    
    public interface Environment<S, A> {
        S getCurrentState();
        List<A> getAvailableActions(S state);
        StepResult<S> takeAction(A action);
        void reset();
        boolean isTerminal(S state);
    }
    
    // MARK: - Data Structures
    
    public static class StepResult<S> {
        public final S nextState;
        public final double reward;
        public final boolean isTerminal;
        
        public StepResult(S nextState, double reward, boolean isTerminal) {
            this.nextState = nextState;
            this.reward = reward;
            this.isTerminal = isTerminal;
        }
    }
    
    public static class QTable<S, A> implements Serializable {
        private final Map<S, Map<A, Double>> table;
        private final double defaultValue;
        
        public QTable(double defaultValue) {
            this.table = new HashMap<>();
            this.defaultValue = defaultValue;
        }
        
        public double getQValue(S state, A action) {
            return table.getOrDefault(state, new HashMap<>())
                       .getOrDefault(action, defaultValue);
        }
        
        public void setQValue(S state, A action, double value) {
            table.computeIfAbsent(state, k -> new HashMap<>())
                 .put(action, value);
        }
        
        public A getBestAction(S state, List<A> availableActions) {
            if (availableActions.isEmpty()) return null;
            
            return availableActions.stream()
                .max(Comparator.comparingDouble(action -> getQValue(state, action)))
                .orElse(availableActions.get(0));
        }
        
        public double getMaxQValue(S state, List<A> availableActions) {
            if (availableActions.isEmpty()) return defaultValue;
            
            return availableActions.stream()
                .mapToDouble(action -> getQValue(state, action))
                .max()
                .orElse(defaultValue);
        }
        
        public Map<A, Double> getActionValues(S state) {
            return Collections.unmodifiableMap(table.getOrDefault(state, new HashMap<>()));
        }
        
        public void merge(QTable<S, A> other, double ratio) {
            other.table.forEach((state, actionMap) -> {
                Map<A, Double> currentActions = table.computeIfAbsent(state, k -> new HashMap<>());
                actionMap.forEach((action, value) -> {
                    double current = currentActions.getOrDefault(action, defaultValue);
                    currentActions.put(action, current * (1 - ratio) + value * ratio);
                });
            });
        }
    }
    
    // MARK: - Core Q-Learning Algorithm
    
    public static class QLearning<S, A> {
        private final QTable<S, A> qTable;
        private final double learningRate;
        private final double discountFactor;
        private final double explorationRate;
        private final double explorationDecay;
        private final double minExplorationRate;
        
        private int trainingEpisodes;
        
        public QLearning(double learningRate, double discountFactor, 
                        double initialExplorationRate, double explorationDecay,
                        double minExplorationRate, double initialQValue) {
            this.qTable = new QTable<>(initialQValue);
            this.learningRate = learningRate;
            this.discountFactor = discountFactor;
            this.explorationRate = initialExplorationRate;
            this.explorationDecay = explorationDecay;
            this.minExplorationRate = minExplorationRate;
            this.trainingEpisodes = 0;
        }
        
        public A chooseAction(S state, List<A> availableActions) {
            if (availableActions.isEmpty()) {
                return null;
            }
            
            // Epsilon-greedy policy
            if (ThreadLocalRandom.current().nextDouble() < explorationRate) {
                // Exploration: random action
                return availableActions.get(
                    ThreadLocalRandom.current().nextInt(availableActions.size())
                );
            } else {
                // Exploitation: best known action
                return qTable.getBestAction(state, availableActions);
            }
        }
        
        public void update(S state, A action, double reward, S nextState, 
                          List<A> nextAvailableActions) {
            double currentQ = qTable.getQValue(state, action);
            double maxNextQ = qTable.getMaxQValue(nextState, nextAvailableActions);
            
            // Q-learning update rule
            double newQ = currentQ + learningRate * (
                reward + discountFactor * maxNextQ - currentQ
            );
            
            qTable.setQValue(state, action, newQ);
        }
        
        public void decayExploration() {
            trainingEpisodes++;
            explorationRate = Math.max(
                minExplorationRate,
                explorationRate * explorationDecay
            );
        }
        
        public Policy<S, A> createGreedyPolicy() {
            return state -> qTable.getBestAction(state, 
                Collections.emptyList()); // Will be provided by environment
        }
        
        // MARK: - Getters
        public double getExplorationRate() { return explorationRate; }
        public int getTrainingEpisodes() { return trainingEpisodes; }
        public QTable<S, A> getQTable() { return qTable; }
    }
    
    // MARK: - Example Environment: Grid World
    
    public static class GridWorld implements Environment<GridState, GridAction> {
        private final int width;
        private final int height;
        private GridState currentState;
        private final Set<GridState> obstacles;
        private final GridState goalState;
        private final double goalReward;
        private final double obstaclePenalty;
        private final double stepPenalty;
        
        public GridWorld(int width, int height, GridState start, GridState goal,
                        Set<GridState> obstacles, double goalReward, 
                        double obstaclePenalty, double stepPenalty) {
            this.width = width;
            this.height = height;
            this.currentState = start;
            this.goalState = goal;
            this.obstacles = obstacles;
            this.goalReward = goalReward;
            this.obstaclePenalty = obstaclePenalty;
            this.stepPenalty = stepPenalty;
        }
        
        @Override
        public GridState getCurrentState() {
            return currentState;
        }
        
        @Override
        public List<GridAction> getAvailableActions(GridState state) {
            List<GridAction> actions = new ArrayList<>();
            
            // Check all possible movements
            for (GridAction action : GridAction.values()) {
                GridState nextState = action.apply(state);
                if (isValidState(nextState)) {
                    actions.add(action);
                }
            }
            
            return actions;
        }
        
        @Override
        public StepResult<GridState> takeAction(GridAction action) {
            GridState nextState = action.apply(currentState);
            double reward = stepPenalty;
            boolean terminal = false;
            
            if (!isValidState(nextState)) {
                // Hit wall - stay in current state with penalty
                nextState = currentState;
                reward = obstaclePenalty;
            } else if (obstacles.contains(nextState)) {
                // Hit obstacle
                reward = obstaclePenalty;
                terminal = false;
            } else if (nextState.equals(goalState)) {
                // Reached goal
                reward = goalReward;
                terminal = true;
            }
            
            currentState = nextState;
            return new StepResult<>(nextState, reward, terminal);
        }
        
        @Override
        public void reset() {
            // Reset to random start position (not goal or obstacle)
            GridState newStart;
            do {
                newStart = new GridState(
                    ThreadLocalRandom.current().nextInt(width),
                    ThreadLocalRandom.current().nextInt(height)
                );
            } while (newStart.equals(goalState) || obstacles.contains(newStart));
            
            currentState = newStart;
        }
        
        @Override
        public boolean isTerminal(GridState state) {
            return state.equals(goalState);
        }
        
        private boolean isValidState(GridState state) {
            return state.x >= 0 && state.x < width && 
                   state.y >= 0 && state.y < height;
        }
    }
    
    // MARK: - Grid World Components
    
    public static class GridState {
        public final int x, y;
        
        public GridState(int x, int y) {
            this.x = x;
            this.y = y;
        }
        
        @Override
        public boolean equals(Object o) {
            if (this == o) return true;
            if (o == null || getClass() != o.getClass()) return false;
            GridState gridState = (GridState) o;
            return x == gridState.x && y == gridState.y;
        }
        
        @Override
        public int hashCode() {
            return Objects.hash(x, y);
        }
        
        @Override
        public String toString() {
            return String.format("(%d, %d)", x, y);
        }
    }
    
    public enum GridAction {
        UP(0, -1), DOWN(0, 1), LEFT(-1, 0), RIGHT(1, 0);
        
        private final int dx, dy;
        
        GridAction(int dx, int dy) {
            this.dx = dx;
            this.dy = dy;
        }
        
        public GridState apply(GridState state) {
            return new GridState(state.x + dx, state.y + dy);
        }
        
        public static GridAction random() {
            GridAction[] actions = values();
            return actions[ThreadLocalRandom.current().nextInt(actions.length)];
        }
    }
    
    // MARK: - Training and Evaluation
    
    public static class TrainingResult<S, A> {
        public final List<Double> episodeRewards;
        public final List<Double> explorationRates;
        public final QTable<S, A> trainedQTable;
        public final Policy<S, A> optimalPolicy;
        
        public TrainingResult(List<Double> episodeRewards, List<Double> explorationRates,
                            QTable<S, A> trainedQTable, Policy<S, A> optimalPolicy) {
            this.episodeRewards = episodeRewards;
            this.explorationRates = explorationRates;
            this.trainedQTable = trainedQTable;
            this.optimalPolicy = optimalPolicy;
        }
    }
    
    public static <S, A> TrainingResult<S, A> trainQLearning(
            QLearning<S, A> qLearning, Environment<S, A> environment,
            int totalEpisodes, int maxStepsPerEpisode) {
        
        List<Double> episodeRewards = new ArrayList<>();
        List<Double> explorationRates = new ArrayList<>();
        
        for (int episode = 0; episode < totalEpisodes; episode++) {
            environment.reset();
            S currentState = environment.getCurrentState();
            double totalReward = 0;
            int steps = 0;
            
            while (steps < maxStepsPerEpisode && !environment.isTerminal(currentState)) {
                // Choose and take action
                List<A> availableActions = environment.getAvailableActions(currentState);
                A action = qLearning.chooseAction(currentState, availableActions);
                
                StepResult<S> result = environment.takeAction(action);
                
                // Q-learning update
                List<A> nextActions = environment.getAvailableActions(result.nextState);
                qLearning.update(currentState, action, result.reward, 
                               result.nextState, nextActions);
                
                totalReward += result.reward;
                currentState = result.nextState;
                steps++;
                
                if (result.isTerminal) {
                    break;
                }
            }
            
            // Record metrics
            episodeRewards.add(totalReward);
            explorationRates.add(qLearning.getExplorationRate());
            
            // Decay exploration rate
            qLearning.decayExploration();
            
            // Progress reporting
            if ((episode + 1) % (totalEpisodes / 10) == 0) {
                System.out.printf("Episode %d: Reward=%.2f, Exploration=%.3f%n",
                    episode + 1, totalReward, qLearning.getExplorationRate());
            }
        }
        
        Policy<S, A> optimalPolicy = qLearning.createGreedyPolicy();
        return new TrainingResult<>(episodeRewards, explorationRates, 
                                  qLearning.getQTable(), optimalPolicy);
    }
    
    // MARK: - Demonstration
    
    public static void demonstrateGridWorld() {
        System.out.println("=== Reinforcement Learning: Grid World Demo ===");
        
        // Create environment: 5x5 grid with obstacles and goal
        Set<GridState> obstacles = Set.of(
            new GridState(1, 1), new GridState(2, 2), new GridState(3, 1)
        );
        GridState startState = new GridState(0, 0);
        GridState goalState = new GridState(4, 4);
        
        GridWorld environment = new GridWorld(5, 5, startState, goalState,
            obstacles, 10.0, -5.0, -0.1);
        
        // Configure Q-learning
        QLearning<GridState, GridAction> qLearning = new QLearning<>(
            0.1,    // learning rate
            0.9,    // discount factor
            1.0,    // initial exploration
            0.995,  // exploration decay
            0.01,   // min exploration
            0.0     // initial Q value
        );
        
        // Train the agent
        int totalEpisodes = 1000;
        int maxSteps = 100;
        
        TrainingResult<GridState, GridAction> result = 
            trainQLearning(qLearning, environment, totalEpisodes, maxSteps);
        
        // Demonstrate learned policy
        System.out.println("\n=== Learned Policy Demonstration ===");
        environment.reset();
        GridState currentState = environment.getCurrentState();
        List<GridAction> path = new ArrayList<>();
        
        System.out.println("Start: " + currentState);
        
        for (int step = 0; step < 20; step++) {
            List<GridAction> availableActions = environment.getAvailableActions(currentState);
            GridAction action = result.optimalPolicy.chooseAction(currentState);
            
            StepResult<GridState> stepResult = environment.takeAction(action);
            path.add(action);
            currentState = stepResult.nextState;
            
            System.out.printf("Step %d: %s -> %s (Reward: %.1f)%n",
                step + 1, action, currentState, stepResult.reward);
            
            if (stepResult.isTerminal) {
                System.out.println("GOAL REACHED!");
                break;
            }
        }
        
        // Show Q-values for start state
        System.out.println("\n=== Q-Values for Start State ===");
        Map<GridAction, Double> startQValues = result.trainedQTable.getActionValues(startState);
        startQValues.forEach((action, value) -> 
            System.out.printf("%s: %.3f%n", action, value));
    }
    
    // MARK: - Advanced: Deep Q-Network Interface (Conceptual)
    
    public interface DeepQNetwork<S> {
        double[] predictQValues(S state);
        void train(S[] states, int[] actions, double[] targets);
        void updateTargetNetwork();
    }
    
    public static void main(String[] args) {
        // Run demonstration
        demonstrateGridWorld();
        
        // Additional RL concepts could be demonstrated here:
        // - SARSA vs Q-learning comparison
        // - Function approximation with neural networks
        // - Policy gradient methods
        // - Multi-armed bandit problems
    }
}