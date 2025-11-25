import java.io.*;
import java.nio.file.*;
import java.util.*;
import java.util.zip.*;
import java.nio.charset.StandardCharsets;
import java.util.stream.Collectors;

/**
 * Comprehensive Java I/O Streams demonstration
 * Covers all major stream types and patterns
 */
public class IOStreamExample {
    
    // MARK: - Byte Stream Fundamentals
    
    public static void demonstrateByteStreams() throws IOException {
        System.out.println("=== Byte Streams Demonstration ===");
        
        String testData = "Hello, Java I/O Streams! 🚀\nThis is a test message.";
        String filename = "byte_stream_demo.dat";
        
        // Writing bytes to file
        try (FileOutputStream fos = new FileOutputStream(filename)) {
            byte[] bytes = testData.getBytes(StandardCharsets.UTF_8);
            fos.write(bytes);
            System.out.println("Written " + bytes.length + " bytes to " + filename);
        }
        
        // Reading bytes from file
        try (FileInputStream fis = new FileInputStream(filename)) {
            byte[] buffer = new byte[1024];
            int bytesRead = fis.read(buffer);
            String readData = new String(buffer, 0, bytesRead, StandardCharsets.UTF_8);
            System.out.println("Read data: " + readData);
        }
        
        // Buffered streams for better performance
        try (BufferedInputStream bis = new BufferedInputStream(
                new FileInputStream(filename));
             BufferedOutputStream bos = new BufferedOutputStream(
                new FileOutputStream("buffered_copy.dat"))) {
            
            byte[] buffer = new byte[8192]; // 8KB buffer
            int bytesRead;
            while ((bytesRead = bis.read(buffer)) != -1) {
                bos.write(buffer, 0, bytesRead);
            }
            System.out.println("Buffered copy completed");
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(filename));
        Files.deleteIfExists(Paths.get("buffered_copy.dat"));
    }
    
    // MARK: - Character Streams and Text Processing
    
    public static void demonstrateCharacterStreams() throws IOException {
        System.out.println("\n=== Character Streams Demonstration ===");
        
        String textFile = "text_demo.txt";
        List<String> lines = Arrays.asList(
            "Line 1: Character streams handle text efficiently",
            "Line 2: They automatically handle character encoding",
            "Line 3: UTF-8, UTF-16, and other encodings are supported",
            "Line 4: 中文测试 - Chinese characters",
            "Line 5: 🎉 Emoji and special symbols"
        );
        
        // Writing text with FileWriter
        try (FileWriter writer = new FileWriter(textFile, StandardCharsets.UTF_8)) {
            for (String line : lines) {
                writer.write(line + "\n");
            }
            System.out.println("Written " + lines.size() + " lines to " + textFile);
        }
        
        // Reading text with FileReader
        try (FileReader reader = new FileReader(textFile, StandardCharsets.UTF_8);
             BufferedReader bufferedReader = new BufferedReader(reader)) {
            
            System.out.println("File content:");
            String line;
            while ((line = bufferedReader.readLine()) != null) {
                System.out.println("  " + line);
            }
        }
        
        // Using PrintWriter for formatted output
        try (PrintWriter pw = new PrintWriter(new FileWriter("formatted_output.txt"))) {
            pw.printf("Formatted output at %tF %<tT%n", new Date());
            pw.println("Price: $%.2f", 19.999);
            pw.printf("Hex: 0x%X, Scientific: %E%n", 255, 1234567.89);
            System.out.println("Formatted output written");
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(textFile));
        Files.deleteIfExists(Paths.get("formatted_output.txt"));
    }
    
    // MARK: - Data Streams for Primitive Types
    
    public static void demonstrateDataStreams() throws IOException {
        System.out.println("\n=== Data Streams Demonstration ===");
        
        String dataFile = "primitives.dat";
        
        // Writing primitive types
        try (DataOutputStream dos = new DataOutputStream(
                new BufferedOutputStream(new FileOutputStream(dataFile)))) {
            
            dos.writeBoolean(true);
            dos.writeByte(65); // 'A'
            dos.writeChar('J');
            dos.writeDouble(Math.PI);
            dos.writeFloat(2.718f);
            dos.writeInt(42);
            dos.writeLong(123456789L);
            dos.writeUTF("Hello Data Stream!");
            
            System.out.println("Primitive types written to " + dataFile);
        }
        
        // Reading primitive types
        try (DataInputStream dis = new DataInputStream(
                new BufferedInputStream(new FileInputStream(dataFile)))) {
            
            System.out.println("Reading primitive types:");
            System.out.println("  Boolean: " + dis.readBoolean());
            System.out.println("  Byte: " + dis.readByte() + " (char: " + (char) dis.readByte() + ")");
            System.out.println("  Char: " + dis.readChar());
            System.out.println("  Double: " + dis.readDouble());
            System.out.println("  Float: " + dis.readFloat());
            System.out.println("  Int: " + dis.readInt());
            System.out.println("  Long: " + dis.readLong());
            System.out.println("  UTF: " + dis.readUTF());
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(dataFile));
    }
    
    // MARK: - Object Serialization Streams
    
    public static class Person implements Serializable {
        private static final long serialVersionUID = 1L;
        
        private final String name;
        private final int age;
        private final transient String password; // transient = not serialized
        private final Date birthDate;
        
        public Person(String name, int age, String password, Date birthDate) {
            this.name = name;
            this.age = age;
            this.password = password;
            this.birthDate = birthDate;
        }
        
        @Override
        public String toString() {
            return String.format("Person{name='%s', age=%d, password=%s, birthDate=%s}",
                name, age, (password != null ? "***" : "null"), birthDate);
        }
    }
    
    public static void demonstrateObjectStreams() throws IOException, ClassNotFoundException {
        System.out.println("\n=== Object Streams Demonstration ===");
        
        String objectFile = "person.ser";
        Person originalPerson = new Person("Alice", 30, "secret123", new Date());
        
        // Serialize object
        try (ObjectOutputStream oos = new ObjectOutputStream(
                new FileOutputStream(objectFile))) {
            oos.writeObject(originalPerson);
            System.out.println("Serialized: " + originalPerson);
        }
        
        // Deserialize object
        try (ObjectInputStream ois = new ObjectInputStream(
                new FileInputStream(objectFile))) {
            Person deserializedPerson = (Person) ois.readObject();
            System.out.println("Deserialized: " + deserializedPerson);
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(objectFile));
    }
    
    // MARK: - Advanced Stream Operations
    
    public static void demonstrateAdvancedStreams() throws IOException {
        System.out.println("\n=== Advanced Stream Operations ===");
        
        // Piped Streams for inter-thread communication
        PipedOutputStream pos = new PipedOutputStream();
        PipedInputStream pis = new PipedInputStream();
        pos.connect(pis);
        
        Thread writerThread = new Thread(() -> {
            try (DataOutputStream dos = new DataOutputStream(pos)) {
                for (int i = 0; i < 5; i++) {
                    dos.writeUTF("Message " + i);
                    Thread.sleep(100);
                }
                dos.writeUTF("END");
            } catch (Exception e) {
                e.printStackTrace();
            }
        });
        
        Thread readerThread = new Thread(() -> {
            try (DataInputStream dis = new DataInputStream(pis)) {
                String message;
                while (!(message = dis.readUTF()).equals("END")) {
                    System.out.println("Piped stream received: " + message);
                }
            } catch (Exception e) {
                e.printStackTrace();
            }
        });
        
        readerThread.start();
        writerThread.start();
        
        try {
            writerThread.join();
            readerThread.join();
        } catch (InterruptedException e) {
            Thread.currentThread().interrupt();
        }
        
        // SequenceInputStream example
        String file1 = "seq1.txt";
        String file2 = "seq2.txt";
        
        Files.write(Paths.get(file1), "Content from file 1\n".getBytes());
        Files.write(Paths.get(file2), "Content from file 2\n".getBytes());
        
        try (SequenceInputStream sis = new SequenceInputStream(
                new FileInputStream(file1), new FileInputStream(file2));
             BufferedReader reader = new BufferedReader(new InputStreamReader(sis))) {
            
            System.out.println("SequenceInputStream output:");
            reader.lines().forEach(line -> System.out.println("  " + line));
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(file1));
        Files.deleteIfExists(Paths.get(file2));
    }
    
    // MARK: - Compression Streams
    
    public static void demonstrateCompressionStreams() throws IOException {
        System.out.println("\n=== Compression Streams Demonstration ===");
        
        String originalFile = "large_content.txt";
        String compressedFile = "compressed.gz";
        
        // Create a file with substantial content
        try (PrintWriter pw = new PrintWriter(new FileWriter(originalFile))) {
            for (int i = 0; i < 1000; i++) {
                pw.printf("Line %04d: This is some repetitive content for compression testing.%n", i);
            }
        }
        
        long originalSize = Files.size(Paths.get(originalFile));
        
        // Compress with GZIP
        try (FileInputStream fis = new FileInputStream(originalFile);
             FileOutputStream fos = new FileOutputStream(compressedFile);
             GZIPOutputStream gzos = new GZIPOutputStream(fos)) {
            
            byte[] buffer = new byte[8192];
            int bytesRead;
            while ((bytesRead = fis.read(buffer)) != -1) {
                gzos.write(buffer, 0, bytesRead);
            }
        }
        
        long compressedSize = Files.size(Paths.get(compressedFile));
        double compressionRatio = (1.0 - (double) compressedSize / originalSize) * 100;
        
        System.out.printf("Compression results:%n");
        System.out.printf("  Original size: %,d bytes%n", originalSize);
        System.out.printf("  Compressed size: %,d bytes%n", compressedSize);
        System.out.printf("  Compression ratio: %.1f%%%n", compressionRatio);
        
        // Decompress
        try (GZIPInputStream gzis = new GZIPInputStream(new FileInputStream(compressedFile));
             BufferedReader reader = new BufferedReader(new InputStreamReader(gzis))) {
            
            System.out.println("First few lines after decompression:");
            reader.lines().limit(3).forEach(line -> System.out.println("  " + line));
        }
        
        // Clean up
        Files.deleteIfExists(Paths.get(originalFile));
        Files.deleteIfExists(Paths.get(compressedFile));
    }
    
    // MARK: - NIO.2 Path and Files Integration
    
    public static void demonstrateNIO2Integration() throws IOException {
        System.out.println("\n=== NIO.2 Integration Demonstration ===");
        
        // Using Paths with traditional streams
        Path nioFile = Paths.get("nio_integration.txt");
        
        // Writing with Files.newBufferedWriter
        try (BufferedWriter writer = Files.newBufferedWriter(nioFile, StandardCharsets.UTF_8)) {
            writer.write("Line 1: NIO.2 integration\n");
            writer.write("Line 2: Efficient file operations\n");
            writer.write("Line 3: Better performance than traditional I/O\n");
        }
        
        // Reading with Files.lines (Stream API)
        System.out.println("Reading with Files.lines():");
        try (Stream<String> lines = Files.lines(nioFile, StandardCharsets.UTF_8)) {
            lines.map(line -> "  " + line)
                 .forEach(System.out::println);
        }
        
        // Copy with Files.copy
        Path copyPath = Paths.get("nio_copy.txt");
        Files.copy(nioFile, copyPath, StandardCopyOption.REPLACE_EXISTING);
        System.out.println("File copied using Files.copy()");
        
        // Reading all bytes
        byte[] allBytes = Files.readAllBytes(nioFile);
        System.out.printf("Read %,d bytes using Files.readAllBytes()%n", allBytes.length);
        
        // Clean up
        Files.deleteIfExists(nioFile);
        Files.deleteIfExists(copyPath);
    }
    
    // MARK: - Real-world Example: Configuration File Handler
    
    public static class ConfigFileHandler {
        private final Path configPath;
        private final Properties properties;
        
        public ConfigFileHandler(String filename) throws IOException {
            this.configPath = Paths.get(filename);
            this.properties = new Properties();
            
            if (Files.exists(configPath)) {
                loadConfig();
            } else {
                createDefaultConfig();
            }
        }
        
        private void loadConfig() throws IOException {
            try (InputStream is = Files.newInputStream(configPath)) {
                properties.load(is);
            }
        }
        
        private void createDefaultConfig() throws IOException {
            properties.setProperty("server.host", "localhost");
            properties.setProperty("server.port", "8080");
            properties.setProperty("database.url", "jdbc:mysql://localhost:3306/app");
            properties.setProperty("cache.enabled", "true");
            properties.setProperty("log.level", "INFO");
            
            saveConfig();
        }
        
        public void saveConfig() throws IOException {
            try (OutputStream os = Files.newOutputStream(configPath);
                 Writer writer = new OutputStreamWriter(os, StandardCharsets.UTF_8)) {
                
                properties.store(writer, "Application Configuration");
            }
        }
        
        public String getProperty(String key) {
            return properties.getProperty(key);
        }
        
        public void setProperty(String key, String value) {
            properties.setProperty(key, value);
        }
        
        public void displayConfig() {
            System.out.println("Current configuration:");
            properties.forEach((key, value) -> 
                System.out.printf("  %s = %s%n", key, value));
        }
    }
    
    public static void demonstrateConfigHandler() throws IOException {
        System.out.println("\n=== Real-world Example: Config File Handler ===");
        
        ConfigFileHandler configHandler = new ConfigFileHandler("app.config");
        configHandler.displayConfig();
        
        // Modify and save
        configHandler.setProperty("server.port", "9090");
        configHandler.setProperty("feature.new", "enabled");
        configHandler.saveConfig();
        
        System.out.println("\nAfter modifications:");
        configHandler.displayConfig();
        
        // Clean up
        Files.deleteIfExists(Paths.get("app.config"));
    }
    
    // MARK: - Error Handling and Best Practices
    
    public static void demonstrateErrorHandling() {
        System.out.println("\n=== Error Handling and Best Practices ===");
        
        // Try-with-resources demonstration
        String tempFile = "temp_resource.txt";
        
        // Proper resource management with try-with-resources
        try (FileWriter fw = new FileWriter(tempFile);
             BufferedWriter bw = new BufferedWriter(fw);
             PrintWriter pw = new PrintWriter(bw)) {
            
            pw.println("This file will be automatically closed");
            pw.println("Even if an exception occurs");
            
            // Simulate an error
            if (System.currentTimeMillis() % 2 == 0) {
                throw new IOException("Simulated I/O error");
            }
            
        } catch (IOException e) {
            System.out.println("Error handled gracefully: " + e.getMessage());
        } finally {
            // Resources are automatically closed by try-with-resources
            try {
                Files.deleteIfExists(Paths.get(tempFile));
            } catch (IOException e) {
                System.out.println("Cleanup error: " + e.getMessage());
            }
        }
        
        // Demonstrating proper stream closing patterns
        System.out.println("Best practices demonstrated:");
        System.out.println("  1. Use try-with-resources for automatic cleanup");
        System.out.println("  2. Prefer buffered streams for better performance");
        System.out.println("  3. Handle character encoding explicitly");
        System.out.println("  4. Use NIO.2 Files class for modern file operations");
        System.out.println("  5. Always close streams in reverse order of opening");
    }
    
    // MARK: - Main Method to Run All Demonstrations
    
    public static void main(String[] args) {
        try {
            demonstrateByteStreams();
            demonstrateCharacterStreams();
            demonstrateDataStreams();
            demonstrateObjectStreams();
            demonstrateAdvancedStreams();
            demonstrateCompressionStreams();
            demonstrateNIO2Integration();
            demonstrateConfigHandler();
            demonstrateErrorHandling();
            
            System.out.println("\n=== All I/O Stream Demonstrations Completed ===");
            
        } catch (Exception e) {
            System.err.println("Demonstration failed: " + e.getMessage());
            e.printStackTrace();
        }
    }
}