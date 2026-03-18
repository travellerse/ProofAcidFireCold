#!/bin/bash
# 使用 Mono 运行 ProofAcidFireCold 测试套件
# Use Mono to run ProofAcidFireCold test suite

set -e  # 遇到错误时退出

# 颜色输出
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

echo -e "${YELLOW}=== ProofAcidFireCold - Mono 测试运行器 ===${NC}"
echo ""

# 检查 Mono 是否已安装
if ! command -v mono &> /dev/null; then
    echo -e "${RED}错误: 未安装 Mono${NC}"
    echo "请运行: sudo pacman -S mono mono-msbuild"
    exit 1
fi

echo -e "${GREEN}✓${NC} Mono 版本: $(mono --version | head -n 1)"
echo ""

# 检查 xUnit 控制台运行器
XUNIT_CONSOLE="$HOME/.nuget/packages/xunit.runner.console/2.6.2/tools/net462/xunit.console.exe"
if [ ! -f "$XUNIT_CONSOLE" ]; then
    echo -e "${YELLOW}安装 xUnit 控制台运行器...${NC}"
    dotnet add tests/ProofAcidFireCold.Tests/ProofAcidFireCold.Tests.csproj \
      package xunit.runner.console --version 2.6.2
    echo -e "${GREEN}✓${NC} xUnit 控制台运行器已安装"
    echo ""
fi

# 编译测试项目
echo -e "${YELLOW}编译测试项目 (net462)...${NC}"
# 使用 Mono 的 msbuild 而不是 dotnet build
# 因为 Linux 上的 .NET SDK 没有 .NET Framework 引用程序集
# LangVersion=latest 让 Mono 使用它支持的最高 C# 版本（通常是 7.3）
msbuild tests/ProofAcidFireCold.Tests/ProofAcidFireCold.Tests.csproj \
  /t:Restore,Build /p:TargetFramework=net462 /p:Configuration=Debug /p:LangVersion=latest /v:minimal

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✓${NC} 测试项目编译成功"
else
    echo -e "${RED}✗${NC} 测试项目编译失败"
    exit 1
fi
echo ""

# 运行测试
echo -e "${YELLOW}使用 Mono 运行测试...${NC}"
echo ""

TEST_DLL="tests/ProofAcidFireCold.Tests/bin/Debug/net462/ProofAcidFireCold.Tests.dll"

mono "$XUNIT_CONSOLE" "$TEST_DLL" -nologo

TEST_RESULT=$?
echo ""

if [ $TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}=== 所有测试通过! ===${NC}"
else
    echo -e "${RED}=== 测试失败 ===${NC}"
fi

exit $TEST_RESULT
