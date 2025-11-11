## Purpose of the Program

**ERROR_FKNK_REALITY_DISINTEGRATED** is an experimental 3D adventure game that immerses players in a surreal, glitch-infested world.  
Players explore distorted environments, solve puzzles, and uncover hidden truths about a collapsing digital reality.

This **repository** is also used for developing and maintaining **separate unit tests** for the game's systems and mechanics, ensuring stability and consistent behavior during gameplay.

**Documentation**
Documentation link: https://Tironosauroo.github.io/ERROR_FKNK_REALITY_DISINTEGRATED/
This project uses **Doxygen** for automatic code documentation generation. The documentation is automatically built and deployed to GitHub Pages through a CI/CD pipeline implemented with GitHub Actions. Every push to the main branch triggers the documentation generation process, ensuring that the online documentation is always up-to-date with the latest code changes.
All core classes and methods are documented using **Doxygen-style comments** with standard tags including @brief, @param, @return, @throws and @example. The documentation covers the inventory queue system, player movement mechanics, and all public APIs with usage examples.
The CI/CD workflow automatically handles documentation generation and deployment on every commit, making the documentation accessible online without manual intervention.
