package main

import (
	"flag"
	"fmt"
	"io/ioutil"
	"log"
	"os"
	"path/filepath"
)

var repo string
var dir string
var file string

func main() {
	parseFlags()
	scan(dir)
}

func parseFlags() {
	flag.StringVar(&repo, "repository", "", "the destination repository")
	flag.StringVar(&dir, "directory", "", "the directory to scan")
	flag.StringVar(&file, "file", "", "the output file")

	flag.Parse()
}

func scan(path string) {
	//path, _ = filepath.Abs(path)

	fmt.Printf("\n%s\n\n", path)

	entries, err := ioutil.ReadDir(path)
	if err != nil {
		log.Fatal(err)
	}

	if len(entries) == 0 {
		fmt.Println("  <EMPTY>")
		return
	}

	dirs := []os.FileInfo{}
	files := []os.FileInfo{}

	for _, f := range entries {
		if f.IsDir() {
			dirs = append(dirs, f)
		} else {
			files = append(files, f)
		}
	}

	for _, d := range dirs {
		fmt.Printf("  %s\t%s\t%-15s\t%s\n", d.ModTime().UTC().Format("2006-01-02 15:04:05 MST"), d.Mode(), "<DIR>", d.Name())
	}

	var totalSize int64

	for _, f := range files {
		totalSize = totalSize + f.Size()
		fmt.Printf("  %s\t%s\t%15d\t%s\n", f.ModTime().UTC().Format("2006-01-02 15:04:05 MST"), f.Mode(), f.Size(), f.Name())
	}

	fmt.Printf("\n%15d File(s)\n%15d Dir(s)\n%15d Bytes\n", len(files), len(dirs), totalSize)

	for _, d := range dirs {
		scan(filepath.Join(path, d.Name()))
	}
}
